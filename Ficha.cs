using Proyecto_1;

public class Ficha
{
    public string Nombre { get; set; }
    public int VelocidadMovimiento { get; set; }
    public int VelocidadOriginal { get; set; } // Almacena la velocidad original
    public string Habilidad { get; set; }
    public int TiempoEnfriamiento { get; set; }
    public int TiempoRestanteEnfriamiento { get; set; }
    public int DuracionHabilidad { get; set; } // Duración del efecto de la habilidad
    public int TiempoRestanteHabilidad { get; set; } // Tiempo restante del efecto de la habilidad
    public bool HabilidadActiva { get; set; } // Indica si la habilidad está activa
    public int Numero { get; set; } // Número para representar la ficha

    public Ficha(string nombre, int velocidadMovimiento, string habilidad, int tiempoEnfriamiento, int duracionHabilidad, int numero)
    {
        Nombre = nombre;
        VelocidadMovimiento = velocidadMovimiento;
        VelocidadOriginal = velocidadMovimiento; // Guarda la velocidad original
        Habilidad = habilidad;
        TiempoEnfriamiento = tiempoEnfriamiento;
        DuracionHabilidad = duracionHabilidad;
        TiempoRestanteEnfriamiento = 0;
        TiempoRestanteHabilidad = 0;
        HabilidadActiva = false;
        Numero = numero;
    }

    public bool PuedeUsarHabilidad()
    {
        return TiempoRestanteEnfriamiento <= 0 && !HabilidadActiva;
    }

    public void UsarHabilidad(Jugador jugador)
    {
        if (PuedeUsarHabilidad())
        {
            HabilidadActiva = true;
            TiempoRestanteHabilidad = DuracionHabilidad;
            Console.WriteLine($"Habilidad '{Habilidad}' activada. Duración: {DuracionHabilidad} turnos.");
            if (Habilidad == "Doble Movimiento")
            {
                // Duplicar la velocidad de todas las fichas del jugador
                foreach (var ficha in jugador.FichasSeleccionadas)
                {
                    ficha.DuplicarVelocidad();
                }
            }
            else if (Habilidad == "Teletransportación")
            {
                // Llamar al método de teletransportación
                Teletransportar(jugador.Tablero, jugador); // Asegúrate de que el jugador tenga una referencia al tablero
            }
            else if (Habilidad == "Rastreador")
            {
                jugador.ObjetivosAlcanzados += 3;
                Console.WriteLine($"{jugador.Nombre} ha ganado 3 objetivos extras!");
            }

        }

    }
    public void DuplicarVelocidad()
    {
        VelocidadMovimiento = VelocidadOriginal * 2; // Duplica la velocidad
    }
    public void RestaurarVelocidad()
    {
        VelocidadMovimiento = VelocidadOriginal; // Restaura la velocidad original
    }
    public void ActualizarEstadoHabilidad(Jugador jugador)
    {
        if (HabilidadActiva)
        {
            TiempoRestanteHabilidad--;
            if (TiempoRestanteHabilidad <= 0)
            {
                HabilidadActiva = false;
                TiempoRestanteEnfriamiento = TiempoEnfriamiento; // Comienza el tiempo de enfriamiento
                Console.WriteLine($"El efecto de la habilidad '{Habilidad}' ha terminado. Tiempo de enfriamiento: {TiempoEnfriamiento} turnos.");
                if (Habilidad == "Doble Movimiento")
                {
                    // Restaurar la velocidad original de todas las fichas del jugador
                    foreach (var ficha in jugador.FichasSeleccionadas)
                    {
                        ficha.RestaurarVelocidad();
                    }
                }
            }
            else if (TiempoRestanteEnfriamiento > 0)
            {
                TiempoRestanteEnfriamiento--;
            }
        }
    }
    public bool EsInmune()
    {
        return HabilidadActiva && Habilidad == "Inmunidad";
    }
    public void Teletransportar(Tablero tablero, Jugador jugador)
    {
        if (Habilidad == "Teletransportación" && PuedeUsarHabilidad())
        {
            Random random = new Random();
            int intentos = 0;
            int maxIntentos = 100; // Límite de intentos para evitar bucles infinitos

            while (intentos < maxIntentos)
            {
                // Generar coordenadas aleatorias dentro del tablero
                int x = random.Next(1, tablero.Tamaño - 1); // Evitar los bordes
                int y = random.Next(1, tablero.Tamaño - 1);

                // Verificar si la posición está vacía
                if (tablero.GetCell(x, y) == ' ')
                {
                    // Limpiar la posición actual de la ficha
                    for (int i = 0; i < tablero.Tamaño; i++)
                    {
                        for (int j = 0; j < tablero.Tamaño; j++)
                        {
                            if (tablero.GetCell(i, j) == Convert.ToChar(Numero.ToString()))
                            {
                                tablero.LimpiarPosicion(i, j);
                                break;
                            }
                        }
                    }

                    // Mover la ficha a la nueva posición
                    tablero.ActualizarPosicionFicha(x, y, Convert.ToChar(Numero.ToString()));
                    Console.WriteLine($"{Nombre} se ha teletransportado a la posición ({x}, {y}).");

                    // Activar la habilidad y comenzar el enfriamiento
                    UsarHabilidad(jugador);
                    return;
                }

                intentos++;
            }

            Console.WriteLine("No se encontró una posición válida para teletransportarse.");
        }
    }
}