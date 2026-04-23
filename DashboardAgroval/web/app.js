const API = "http://localhost:5158/api";

async function cargarDatos() {
  const res = await fetch(`${API}/caseteros`);
  return await res.json();
}

async function cargarFODA(nombre) {
  const res = await fetch(`${API}/foda/${encodeURIComponent(nombre)}`);
  return await res.json();
}

function diasDesde(fecha) {
  const inicio = new Date(fecha);
  const hoy = new Date();
  return Math.floor((hoy - inicio) / (1000*60*60*24));
}

function fase(dias) {
  if (dias <= 10) return "f1";
  if (dias <= 20) return "f2";
  return "f3";
}

function color(dias) {
  if (dias <= 10) return "rojo";
  if (dias <= 20) return "amarillo";
  return "verde";
}

function comentario(nombre, primer, ultimo) {
  if (["María de los Ángeles Castillo Contreras",
       "Placencia Hernández José Miguel",
       "Santiago Dessens Luis Enrique",
       "Cesar Braulio Pereida Monroy"].includes(nombre)) return "BAJA";

  let mejoras = 0, empeoras = 0;

  const campos = ["Limpieza","Calidad","Tecnica","Resistencia"];
  campos.forEach(c => {
    if (ultimo[c] > primer[c]) mejoras++;
    else if (ultimo[c] < primer[c]) empeoras++;
  });

  if (ultimo.Merma < primer.Merma) mejoras++;
  else if (ultimo.Merma > primer.Merma) empeoras++;

  if (mejoras > empeoras) return "Comentario positivo";
  if (empeoras > mejoras) return "Comentario negativo";
  return "Rendimiento detenido";
}

async function render() {
  const tbody = document.getElementById("tbody");
  const fodaContainer = document.getElementById("fodaContainer");

  const data = await cargarDatos();

  const personas = {};
  data.forEach(r => {
    if (!personas[r.Nombre]) personas[r.Nombre] = [];
    personas[r.Nombre].push(r);
  });

  let activos = [];
  let bajas = [];

  for (const nombre in personas) {
    const reportes = personas[nombre];
    const primer = reportes[0];
    const ultimo = reportes[reportes.length - 1];
    const dias = diasDesde(primer.Inicio);

    const reg = {
      nombre,
      dias,
      primer,
      ultimo,
      fase: fase(dias),
      color: color(dias),
      comentario: comentario(nombre, primer, ultimo)
    };

    if (reg.comentario === "BAJA") bajas.push(reg);
    else activos.push(reg);
  }

  activos.sort((a,b)=>b.dias-a.dias);
  bajas.sort((a,b)=>b.dias-a.dias);

  let i = 1;
  activos.forEach(p => {
    tbody.innerHTML += `
      <tr>
        <td>${i++}</td>
        <td>${p.nombre}</td>
        <td><span class="avance ${p.color}">${p.dias} días</span></td>
        <td>${p.ultimo.Cajas}</td>
        <td>${Math.round((p.ultimo.Limpieza+p.ultimo.Calidad+p.ultimo.Tecnica+p.ultimo.Resistencia)/4)}</td>
        <td><span class="fase-tag ${p.fase}">${p.fase.toUpperCase()}</span></td>
        <td>${p.comentario}</td>
      </tr>
    `;
  });

  bajas.forEach(p => {
    tbody.innerHTML += `
      <tr style="opacity:0.6;">
        <td>${i++}</td>
        <td>${p.nombre}</td>
        <td><span class="avance rojo">${p.dias} días</span></td>
        <td>${p.ultimo.Cajas}</td>
        <td>${Math.round((p.ultimo.Limpieza+p.ultimo.Calidad+p.ultimo.Tecnica+p.ultimo.Resistencia)/4)}</td>
        <td><span class="fase-tag f1">F1</span></td>
        <td>BAJA</td>
      </tr>
    `;
  });

  for (const p of [...activos, ...bajas]) {
    const f = await cargarFODA(p.nombre);

    fodaContainer.innerHTML += `
      <div class="foda-box">
        <div class="foda-title">${p.nombre}${p.comentario==="BAJA"?" (BAJA)":""}</div>
        <p><strong>Fortalezas:</strong> ${f.F}</p>
        <p><strong>Oportunidades:</strong> ${f.O}</p>
        <p><strong>Debilidades:</strong> ${f.D}</p>
        <p><strong>Amenazas:</strong> ${f.A}</p>
      </div>
    `;
  }
}

render();

