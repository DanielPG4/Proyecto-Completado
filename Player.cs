using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Proyecto_1
{
    public class Jugador
    {
        public string Nombre { get; set; }
        public char Simbolo { get; set; }
        public List<Ficha> FichasSeleccionadas { get; set; } // Fichas elegidas por el jugador
        public int Puntos { get; set; }
        public bool HabilidadUsadaEsteTurno { get; set; }
        public int ObjetivosAlcanzados { get; set; } // Contador de objetivos alcanzados
        public bool EstaCongelado { get; set; }
        public int TurnosRalentizado { get; set; }
        public ConsoleColor Color { get; set; }
        public Tablero Tablero { get; set; } // Referencia al tablero

        public Jugador(string nombre, char simbolo, ConsoleColor color, Tablero tablero)
        {
            Nombre = nombre;
            Simbolo = simbolo;
            FichasSeleccionadas = new List<Ficha>();
            Puntos = 0;
            ObjetivosAlcanzados = 0; // Inicializa el contador de objetivos
            Color = color; // Asignar color al jugador
            Tablero = tablero; // Inicializar la referencia al tablero
        }

        public void SeleccionarFichas(List<Ficha> todasLasFichas)
        {
            for (int i = 0; i < 2; i++)
            {
                Console.Clear();
                Console.WriteLine($"\nSelecciona la ficha #{i + 1} de las siguientes:\n");
                for (int j = 0; j < todasLasFichas.Count; j++)
                {
                    Console.WriteLine($"{j + 1}: {todasLasFichas[j].Nombre} (Velocidad: {todasLasFichas[j].VelocidadMovimiento})");
                }

                int seleccion = -1;
                bool seleccionValida = false;

                while (!seleccionValida)
                {
                    Console.Write("\nIngresa el número de la ficha: \n");
                    string input = Console.ReadLine()!;

                    if (string.IsNullOrWhiteSpace(input))
                    {
                        Console.WriteLine("Por favor, ingresa un número válido.");
                        continue;
                    }

                    if (int.TryParse(input, out seleccion))
                    {
                        seleccion--; // Ajustamos para que coincida con el índice del array
                        if (seleccion >= 0 && seleccion < todasLasFichas.Count)
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

                FichasSeleccionadas.Add(todasLasFichas[seleccion]);
                todasLasFichas.RemoveAt(seleccion); // Eliminar la ficha seleccionada de la lista
            }       
        }

        public void RecolectarPuntos(int puntos)
        {
            Puntos += puntos;
            VerificarObjetivos(); // Verifica si se ha alcanzado un objetivo
        }

        private void VerificarObjetivos()
        {
            // Lógica para verificar si se alcanza un objetivo.
        
            if (Puntos % 10 == 0) 
            {
                ObjetivosAlcanzados++;
                Console.WriteLine($"{Nombre} ha alcanzado un objetivo! Total de objetivos: {ObjetivosAlcanzados}");
            }
        
            if (ObjetivosAlcanzados == 7)
            {
                Console.WriteLine($"{Nombre} ha alcanzado 7 objetivos! Se abre una salida especial.");
                // Aquí puedes implementar la lógica para abrir una salida en el laberinto.
            }
        }
    

        public void AplicarEfectoTrampa(string tipoTrampa)
        {
            switch(tipoTrampa)
            {
                case "Ralentización"://Funciona super bien con la implementacion y todo 
                    TurnosRalentizado = 2; // Ralentizado por 2 movimientos
                    Console.WriteLine($"{Nombre} ha sido ralentizado por 2 turnos!");
                    Console.WriteLine("Presione cualquier tecla para continuar");
                    Console.ReadKey();
                    break;
                case "Congelación"://Si Funciona 
                    EstaCongelado = true;
                    Console.WriteLine($"{Nombre} ha sido congelado y perderá su próximo turno!");
                    Console.WriteLine("Presione cualquier tecla para continuar");
                    Console.ReadKey();
                    break;
                case "PérdidaPuntos"://Funciona super bien con la implementacion y todo 
                    if (ObjetivosAlcanzados > 0)
                    {
                        ObjetivosAlcanzados--;
                        Console.WriteLine($"{Nombre} ha perdido un objetivo!");
                        Console.WriteLine("Presione cualquier tecla para continuar");
                        Console.ReadKey();
                    }
                    else
                    {
                        Console.WriteLine($"{Nombre} no tiene objetivos para perder.");
                        Console.WriteLine("Presione cualquier tecla para continuar");
                        Console.ReadKey();
                    }
                    break;
            }
        }
    }
}
