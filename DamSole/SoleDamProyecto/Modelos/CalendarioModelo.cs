using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace SoleDamProyecto.Modelos
{
    /// <summary>Colores de la aplicación y funciones para saber si un día es fin de semana.</summary>
    internal static class Paleta
    {
        private static SolidColorBrush B(string hex)
        {
            var b = (SolidColorBrush)new BrushConverter().ConvertFromString(hex);
            b.Freeze();
            return b;
        }

        public static readonly Brush Libre = B("#22A060");
        public static readonly Brush Ocupado = B("#E14040");
        public static readonly Brush Festivo = B("#EAB308");
        public static readonly Brush Finde = B("#5E617A");
        public static readonly Brush TextoFinde = B("#D3D5E4");
        public static readonly Brush Acento = B("#6366F1");
        public static readonly Brush TextoSuave = B("#8C8FA5");
        public static readonly Brush TextoSobreAmarillo = B("#282000");
        public static readonly Brush Hover = B("#99FFFFFF");
        public static readonly Brush TextoDialogo = B("#6B6E85");
        public static readonly Brush Error = B("#C62828");

        /// <summary>true = sábados y domingos no se pueden bloquear (quedan en gris).</summary>
        public const bool FindesNoBloqueables = true;

        public static bool EsFinde(DateTime d)
        {
            return d.DayOfWeek == DayOfWeek.Saturday || d.DayOfWeek == DayOfWeek.Sunday;
        }

        /// <summary>Indica si el día no se puede pulsar ni bloquear (los fines de semana).</summary>
        public static bool NoBloqueable(DateTime d)
        {
            return FindesNoBloqueables && EsFinde(d);
        }

        public static bool EsFestivo(string tipo)
        {
            return string.Equals(tipo, BloqueosCalendarioService.TipoFestivo, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>Devuelve el color de un bloqueo según su tipo (rojo ocupado, amarillo festivo).</summary>
        public static Brush Tipo(string tipo)
        {
            return EsFestivo(tipo) ? Festivo : Ocupado;
        }
    }

    /// <summary>Opción del desplegable "Motivo".</summary>
    public class TipoItem
    {
        public string Nombre { get; set; }
        public string Valor { get; set; }    // valor que se guarda en el servicio
        public Brush Color { get; set; }
    }

    /// <summary>
    /// Un bloqueo del calendario: un rango de fechas [FechaInicio, FechaFin] (ambas incluidas).
    /// Es el equivalente a la entidad BloqueosCalendario del proyecto Spring Boot
    /// (fecha_inicio, fecha_fin, tipo). Un único día es un rango con inicio == fin.
    /// </summary>
    public class Bloqueo
    {
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public string Tipo { get; set; }

        public Bloqueo(DateTime inicio, DateTime fin, string tipo)
        {
            FechaInicio = inicio.Date;
            FechaFin = fin.Date;
            Tipo = tipo;
        }
    }

    /// <summary>
    /// Misma lógica que BloqueosCalendarioService.java, pero en memoria:
    /// no se conecta a ningún servidor ni base de datos.
    /// </summary>
    public class BloqueosCalendarioService
    {
        public const string TipoOcupado = "OCUPADO";
        public const string TipoFestivo = "FESTIVO";

        private readonly List<Bloqueo> _bloqueos = new List<Bloqueo>();

        /// <summary>
        /// Bloquea un rango de fechas (ambas incluidas). Si pisa bloqueos anteriores,
        /// esa parte se sustituye por el nuevo tipo. Los bloqueos contiguos del
        /// mismo tipo se unen en uno solo.
        /// </summary>
        public void BloquearRango(DateTime inicio, DateTime fin, string tipo)
        {
            ValidarRango(inicio, fin);
            DesbloquearRango(inicio, fin);                       // evita solapes
            _bloqueos.Add(new Bloqueo(inicio, fin, tipo));
            Fusionar();
        }

        /// <summary>Bloquea un único día: es un rango donde inicio == fin.</summary>
        public void BloquearDia(DateTime dia, string tipo)
        {
            BloquearRango(dia, dia, tipo);
        }

        /// <summary>Desbloquea un único día.</summary>
        public void DesbloquearDia(DateTime dia)
        {
            DesbloquearRango(dia, dia);
        }

        /// <summary>
        /// Desbloquea un rango [desde, hasta]. Cada bloqueo existente que se solape se
        /// recorta o se parte en dos, o se elimina si queda completamente dentro del rango.
        /// </summary>
        public void DesbloquearRango(DateTime desde, DateTime hasta)
        {
            ValidarRango(desde, hasta);
            desde = desde.Date;
            hasta = hasta.Date;

            List<Bloqueo> afectados = _bloqueos
                .Where(b => b.FechaInicio <= hasta && b.FechaFin >= desde)
                .ToList();

            foreach (Bloqueo b in afectados)
            {
                bool sobraIzquierda = b.FechaInicio < desde;
                bool sobraDerecha = b.FechaFin > hasta;

                if (sobraIzquierda && sobraDerecha)
                {
                    // El hueco está en medio -> partimos el bloqueo en dos
                    DateTime finOriginal = b.FechaFin;
                    b.FechaFin = desde.AddDays(-1);
                    _bloqueos.Add(new Bloqueo(hasta.AddDays(1), finOriginal, b.Tipo));
                }
                else if (sobraIzquierda)
                {
                    b.FechaFin = desde.AddDays(-1);
                }
                else if (sobraDerecha)
                {
                    b.FechaInicio = hasta.AddDays(1);
                }
                else
                {
                    _bloqueos.Remove(b);
                }
            }
        }

        /// <summary>Elimina un bloqueo entero, sin partirlo.</summary>
        public void Eliminar(Bloqueo bloqueo)
        {
            _bloqueos.Remove(bloqueo);
        }

        /// <summary>Devuelve el bloqueo que contiene ese día, o null si está libre.</summary>
        public Bloqueo Buscar(DateTime dia)
        {
            dia = dia.Date;
            foreach (Bloqueo b in _bloqueos)
                if (b.FechaInicio <= dia && b.FechaFin >= dia)
                    return b;
            return null;
        }

        /// <summary>Indica si el día está ocupado o festivo.</summary>
        public bool EstaBloqueado(DateTime dia)
        {
            return Buscar(dia) != null;
        }

        /// <summary>Devuelve todos los bloqueos ordenados por fecha.</summary>
        public List<Bloqueo> Listar()
        {
            return _bloqueos.OrderBy(b => b.FechaInicio).ToList();
        }

        /// <summary>Une en uno solo los bloqueos contiguos del mismo tipo.</summary>
        private void Fusionar()
        {
            List<Bloqueo> ordenados = _bloqueos.OrderBy(b => b.FechaInicio).ToList();
            _bloqueos.Clear();

            Bloqueo actual = null;
            foreach (Bloqueo b in ordenados)
            {
                if (actual != null
                    && string.Equals(actual.Tipo, b.Tipo, StringComparison.OrdinalIgnoreCase)
                    && b.FechaInicio <= actual.FechaFin.AddDays(1))
                {
                    if (b.FechaFin > actual.FechaFin) actual.FechaFin = b.FechaFin;
                }
                else
                {
                    actual = b;
                    _bloqueos.Add(b);
                }
            }
        }

        /// <summary>Lanza un error si la fecha fin es anterior a la de inicio.</summary>
        private static void ValidarRango(DateTime inicio, DateTime fin)
        {
            if (fin.Date < inicio.Date)
                throw new ArgumentException("La fecha fin no puede ser anterior a la fecha inicio");
        }
    }
}