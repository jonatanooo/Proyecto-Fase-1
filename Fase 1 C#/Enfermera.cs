using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.Json;
using Fase_1_C_.Models;

namespace Fase_1_C_
{
    public partial class Enfermera : Form
    {
        public Enfermera()
        {
            InitializeComponent();
        }



        private void Enfermera_Load(object sender, EventArgs e)
        {

        }

        private void materialRadioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void hopeNumeric2_Click(object sender, EventArgs e)
        {

        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void txtnombre_Enter(object sender, EventArgs e)
        {
            // Si el texto es el placeholder, lo borramos y ponemos la letra negra
            if (txtnombre.Text == "Nombres")
            {
                txtnombre.Text = "";
                txtnombre.ForeColor = Color.Black;
            }
        }

        private void txtapellidos_Enter(object sender, EventArgs e)
        {
            // Si el texto es el placeholder, lo borramos y ponemos la letra negra
            if (txtapellidos.Text == "Apellidos")
            {
                txtapellidos.Text = "";
                txtapellidos.ForeColor = Color.Black;
            }
        }

        private void label15_Click(object sender, EventArgs e)
        {

        }

        private void txtdui_Enter(object sender, EventArgs e)
        {
            if (txtdui.Text == "Ingrese numero de DUI")
            {
                txtdui.Text = "";
                txtdui.ForeColor = Color.Black;
            }
        }

        private void btningresar_Click(object sender, EventArgs e)
        {
            //Boton para ingresar los datos

            using (var db = new TriajeClinicoContext())
            {
                try
                {
                    //Fase 1: Paciente (Buscar si existe, si no crearlo)
                    string duiIngresado = txtdui.Text;

                    //EF busca en la tabla "Pacientes" si hay alguno con ese DUI
                    var pacienteActual = db.Pacientes.FirstOrDefault(p => p.DuiPaciente == duiIngresado);

                    //Si el pacienteActual es null, lo insertamos
                    if (pacienteActual == null)
                    {
                        pacienteActual = new Paciente
                        {
                            DuiPaciente = duiIngresado,
                            Nombres = txtnombre.Text,
                            Apellidos = txtapellidos.Text,
                            FechaNacimiento = DateOnly.FromDateTime(dateedad.Value),
                            Genero = cbgenero.Text
                        };
                        //Añade los datos anteriores
                        db.Pacientes.Add(pacienteActual);
                        db.SaveChanges();
                    }

                    //Fase 2 Sintomas Criticos Utilizando JSON
                    var sintomas = new Dictionary<string, bool>
                    {
                        {"Desmayo", rdbDesmayoSi.Checked},
                        {"HemorragiaIncontrolable", rdbHemorragiaSi.Checked},
                        {"DificultadRespiratoriaSevera", rdbDificultadadResSi.Checked},
                        {"TraumaCraneoencefalico", rdbTraumaCraneoSi.Checked},
                        {"DolorPecho", rdbDolorPechoSi.Checked}
                    };
                    //El serializer sirva para empacaar, es decir es el proceso de tomar un objeto y convertirlo en una cade de texto plana con formato JSON
                    string jsonSintomas = JsonSerializer.Serialize(sintomas);

                    //Fase 3 Guardar el registro de TRIAJE
                    var nuevoRegistro = new RegistrosTriaje
                    {
                        IdPaciente = pacienteActual.IdPaciente,
                        IdUsuarioEnfermera = 2,//Ejemplo de ID Enfermera de muestra
                        FechaHoraIngreso = DateTime.Now,

                        MotivoConsulta = cbmotivo.Text,
                        EstadoMental = cbestadomental.Text,
                        MecanismoLesion = cbmecanismo.Text,

                        //Convertir el numevolucion en enteros
                        TiempoEvolucion = Convert.ToInt32(numevolucion.ValueNumber),

                        SintomasCriticosJson = jsonSintomas,
                        EstadoAtencion = "En Sala de Espera",

                        Temperatura = Convert.ToDecimal(numtemperatura.ValueNumber),
                        NivelDolor = Convert.ToInt32(numdolor.ValueNumber),
                        SaturacionO2 = Convert.ToInt32(numoxigeno.ValueNumber),

                        //Espacio para asignar el nivel de gravedad
                        //
                        //////////////////////////////////////////
                        ///
                    };

                    db.RegistrosTriajes.Add(nuevoRegistro);
                    db.SaveChanges();

                    MessageBox.Show("El paciente se ha ingresado en la clasificacion de triaje", "Exito");

                }
                catch (Exception ex)
                {
                    // Extraemos el error principal
                    string mensajeError = ex.Message;

                    // Si hay un "InnerException" (el detalle real de SQL Server), lo agregamos
                    if (ex.InnerException != null)
                    {
                        mensajeError += "\n\nDetalle de SQL Server:\n" + ex.InnerException.Message;
                    }

                    MessageBox.Show("Hubo un problema al guardar:\n\n" + mensajeError, "Error de Base de Datos");
                }
            }
        }

        private void btncancelar_Click(object sender, EventArgs e)
        {
            Login abrir = new Login();
            abrir.Show();
            this.Hide();
        }
    }
}
