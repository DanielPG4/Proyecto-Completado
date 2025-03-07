using System;
using System.Collections.Generic;
using Proyecto_1;

class Program
{
    static void Main(string[] args)
    {
        int tamaño = 31;
        Tablero tablero = new Tablero(tamaño);

        List<Ficha> todasLasFichas = new List<Ficha>
        {
            new Ficha("Ficha Lenta", 5, "Rastreador", 7, 1, 1), // Duración: 3 turnos, Enfriamiento: 5 turnos
            new Ficha("Ficha Equilibrada", 5, "Teletransportación", 4, 1, 2), // Duración: 2 turnos, Enfriamiento: 3 turnos
            new Ficha("Ficha Rápida", 5, "Salto", 2, 1, 3), // Duración: 1 turno, Enfriamiento: 2 turnos
            new Ficha("Ficha Poderosa", 5, "Inmunidad", 4, 2, 4), // Duración: 2 turnos, Enfriamiento: 4 turnos
            new Ficha("Ficha Ágil", 5, "Doble Movimiento", 6, 2, 5) // Duración: 3 turnos, Enfriamiento: 6 turnos
        };

        string nombre1 = ObtenerNombreJugador("Ingrese el nombre del Jugador 1:");
        Jugador jugador1 = new Jugador(nombre1, 'A', ConsoleColor.Green, tablero);

        string nombre2 = ObtenerNombreJugador("Ingrese el nombre del Jugador 2:");
        Jugador jugador2 = new Jugador(nombre2, 'B', ConsoleColor.Magenta, tablero);

        jugador1.SeleccionarFichas(todasLasFichas);
        jugador2.SeleccionarFichas(todasLasFichas);

        List<Ficha> listica = new List<Ficha>();
        listica.AddRange(jugador1.FichasSeleccionadas);
        listica.AddRange(jugador2.FichasSeleccionadas);

        tablero.ColocarFichasAleatorias(listica);
        tablero.GenerarTrampas(10); // Genera 10 trampas en el tablero

        while (true)
        {
            RealizarTurno(jugador1, tablero, jugador2);
            if (jugador1.ObjetivosAlcanzados >= 7)
            {
                Console.WriteLine("Has alcanzado los objetivos necesarios!");
                Console.WriteLine($"¡Felicidades, has ganado {jugador1.Nombre}!");
                break;
            }
            RealizarTurno(jugador2, tablero, jugador1);
            if (jugador2.ObjetivosAlcanzados >= 7)
            {
                Console.WriteLine("Has alcanzado los objetivos necesarios!");
                Console.WriteLine($"¡Felicidades, has ganado {jugador2.Nombre}!");
                break;
            }
        }
    }

    static string ObtenerNombreJugador(string mensaje)
    {
        string nombre;
        do
        {
            Console.WriteLine(mensaje);
            nombre = Console.ReadLine()?.Trim()!;
            if (string.IsNullOrWhiteSpace(nombre))
            {
                Console.WriteLine("El nombre no puede estar vacío. Inténtalo de nuevo.");
            }
        } while (string.IsNullOrWhiteSpace(nombre));
        return nombre;
    }

    static void RealizarTurno(Jugador jugador, Tablero tablero, Jugador otroJugador)
    {
        tablero.RellenarObjetivos();
        if (jugador.EstaCongelado)
        {
            Console.WriteLine($"{jugador.Nombre} está congelado y pierde este turno.");
            Console.WriteLine("Pulse cualquier tecla para continuar");
            Console.ReadLine();
            jugador.EstaCongelado = false;
            return;
        }
        
        for (int i = 0; i < 5; i++)
        {   
            Console.Clear();
            Console.WriteLine($"\nMovimientos restantes: {5 - i}");
            tablero.MostrarLaberinto(jugador, otroJugador);
            Console.WriteLine($"Turno de {jugador.Nombre}:");
            Console.WriteLine($"Objetivos alcanzados: {jugador.ObjetivosAlcanzados}");
            Console.WriteLine("Selecciona una ficha para mover o usar su habilidad:\n");

            for (int j = 0; j < jugador.FichasSeleccionadas.Count; j++)
            {
                var ficha = jugador.FichasSeleccionadas[j];
                string estadoHabilidad = ficha.HabilidadActiva ? $"Activa ({ficha.TiempoRestanteHabilidad} turnos restantes)" :
                    ficha.TiempoRestanteEnfriamiento > 0 ? $"Enfriamiento ({ficha.TiempoRestanteEnfriamiento} turnos restantes)" : "Lista";
                Console.WriteLine($"{j + 1}: {ficha.Nombre} (Velocidad: {ficha.VelocidadMovimiento}, Habilidad: {ficha.Habilidad}, Estado: {estadoHabilidad})");
            }

            int seleccionFicha = -1;
            bool seleccionValida = false;

            while (!seleccionValida)
            {
                Console.Write("Ingresa el número de la ficha: \n");
                string input = Console.ReadLine()!;

                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.WriteLine("Por favor, ingresa un número válido.");
                    continue;
                }

                if (int.TryParse(input, out seleccionFicha))
                {
                    seleccionFicha--; // Ajustamos para que coincida con el índice del array
                    if (seleccionFicha >= 0 && seleccionFicha < jugador.FichasSeleccionadas.Count)
                    {
                        seleccionValida = true;
                    }
                    else
                    {
                        Console.WriteLine("Número fuera de rango. Por favor, elige un número válido.");
                    }
                }
                else
                {
                    Console.WriteLine("Entrada inválida. Por favor, ingresa un número.");
                }
            }

            char simboloFicha = Convert.ToChar(jugador.FichasSeleccionadas[seleccionFicha].Numero.ToString());
            int posX = -1, posY = -1;

            Ficha fichaSeleccionada = jugador.FichasSeleccionadas[seleccionFicha];

            // Opción para usar la habilidad
            if (fichaSeleccionada.PuedeUsarHabilidad())
            {
                Console.WriteLine($"¿Quieres usar la habilidad '{fichaSeleccionada.Habilidad}'? (s/n)");
                string respuesta = Console.ReadLine()!.ToLower();

                if (respuesta == "s")
                {
                    if (fichaSeleccionada.Habilidad == "Teletransportación")
                    {
                        fichaSeleccionada.Teletransportar(tablero, jugador);
                    }
                    else
                    {
                        fichaSeleccionada.UsarHabilidad(jugador);
                    }
                    jugador.HabilidadUsadaEsteTurno = true; // Marca que ya usó una habilidad este turno
                    
                    Console.ReadKey();
                }
            }

            for (int x = 0; x < tablero.Tamaño; x++)
            {
                for (int y = 0; y < tablero.Tamaño; y++)
                {
                    if (tablero.GetCell(x, y) == simboloFicha)
                    {
                        posX = x;
                        posY = y;
                        break;
                    }
                }
                if (posX != -1) break;
            }

            int cantDePasos = jugador.FichasSeleccionadas[seleccionFicha].VelocidadMovimiento;
            if (jugador.TurnosRalentizado > 0)
            {
                cantDePasos = Math.Max(1, cantDePasos / 2);
                jugador.TurnosRalentizado--;
            }

            while(cantDePasos > 0)
            {
                Console.Clear();
                tablero.MostrarLaberinto(jugador, otroJugador);
                Console.WriteLine("Usa W/A/S/D para mover:");
                char direccion = Console.ReadKey().KeyChar;

                int nuevaX = posX, nuevaY = posY;

                switch (direccion)
                {
                    case 'w': nuevaX -= 1; break;
                    case 's': nuevaX += 1; break;
                    case 'a': nuevaY -= 1; break;
                    case 'd': nuevaY += 1; break;
                    default:
                        Console.WriteLine("\nDirección inválida.");
                        continue;
                }

                if (tablero.EsPosicionValida(nuevaX, nuevaY))
                {
                    if (tablero.RecolectarObjetivo(nuevaX, nuevaY))
                    {
                        jugador.ObjetivosAlcanzados++;
                        Console.WriteLine($"{jugador.Nombre} ha recolectado un objetivo!");
                    }
                    else if (tablero.EsTrampa(nuevaX, nuevaY))
                    {

                        if (fichaSeleccionada.EsInmune())
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"{jugador.Nombre} es inmune a las trampas debido a la habilidad de Inmunidad.");
                            Console.ResetColor();
                        }
                        else
                        {
                            Console.WriteLine($"{jugador.Nombre} ha caído en una trampa!");
                            string[] tiposTrampas = { "Ralentización", "Congelación", "PérdidaPuntos" };
                            string trampaActivada = tiposTrampas[new Random().Next(tiposTrampas.Length)];
                            jugador.AplicarEfectoTrampa(trampaActivada);
                            tablero.LimpiarPosicion(nuevaX, nuevaY);
                        }
                    }

                    tablero.LimpiarPosicion(posX, posY);
                    tablero.ActualizarPosicionFicha(nuevaX, nuevaY, simboloFicha);
                    posX = nuevaX;
                    posY = nuevaY;
                    cantDePasos--;
                }
                else
                {
                    Console.WriteLine("\nMovimiento inválido.");
                }
            }
        }
        // Actualizar el estado de las habilidades al final del turno
        foreach (var ficha in jugador.FichasSeleccionadas)
        {
            ficha.ActualizarEstadoHabilidad(jugador);
        }
    }
}