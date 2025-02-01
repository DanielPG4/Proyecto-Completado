using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Proyecto_1
{
    public class Tablero
    {
        private char[,] laberinto;
        private int tamaño;
        private int objetivosRestantes;

        public int Tamaño => tamaño;

        public Tablero(int tamaño)
        {
            if (tamaño % 2 == 0) tamaño++; 
            this.tamaño = tamaño;
            laberinto = new char[tamaño, tamaño];
            InicializarLaberinto();
            GenerarCamino(1, 1);
            NuevosCaminos(25);
            GenerarObjetivos(20); 
            GenerarTrampas(10);
        }

        private void InicializarLaberinto()
        {
            for (int i = 0; i < tamaño; i++)
            {
                for (int j = 0; j < tamaño; j++)
                {
                    laberinto[i, j] = '█'; // Inicializa todas las celdas como paredes
                }
            }

            laberinto[1, 1] = ' '; // Asegura que la celda inicial esté vacía.
        }    

        private void GenerarCamino(int x, int y)
        {
            laberinto[x, y] = ' '; // Marca la celda como parte del camino   

            int[] dx = { -2, 2, 0, 0 };
            int[] dy = { 0, 0, -2, 2 };  

            Shuffle(dx, dy); // Mezcla las direcciones   

            for (int i = 0; i < dx.Length; i++)
            {
                int nx = x + dx[i];
                int ny = y + dy[i];  

                if (IsInBounds(nx, ny) && laberinto[nx, ny] == '█')
                {
                    laberinto[x + dx[i] / 2, y + dy[i] / 2] = ' '; // Tumba la pared entre celdas
                    GenerarCamino(nx, ny);
                }
            }
        }
        private void NuevosCaminos(int n)
        {
            Random rng = new Random();
            int count = 0;

            while (count < n)
            {
                // Generar coordenadas aleatorias dentro del rango válido (evitando los bordes)
                int x = rng.Next(1, tamaño - 1);
                int y = rng.Next(1, tamaño - 1);

                // Verificar si la celda actual es una pared ('█') y no está en los bordes
            if (laberinto[x, y] == '█')
                {
                    laberinto[x, y] = ' '; // Romper la pared
                    count++; // Incrementar el contador de paredes rotas
                }
            }
        }


        private void Shuffle(int[] dx, int[] dy)
        {
            Random rng = new Random();
            for (int i = dx.Length - 1; i > 0; i--)
            {
                int j = rng.Next(i + 1);
                (dx[i], dx[j]) = (dx[j], dx[i]);
                (dy[i], dy[j]) = (dy[j], dy[i]);
            }
        }    

        private bool IsInBounds(int x, int y)
        {
            return x >= 0 && x < tamaño && y >= 0 && y < tamaño;
        }    

        public void MostrarLaberinto(Jugador jugador1, Jugador jugador2)
        {
        
            Console.WriteLine("╔═══════════════════════════════════════════════════════════════╗");
            for (int i = 0; i < tamaño; i++)
            {
                Console.Write("║ ");
                for (int j = 0; j < tamaño; j++)
                {
                    char celda = laberinto[i, j];
                    if (char.IsDigit(celda)) // Si es una ficha (representada por un número)
                    {
                        int numeroFicha = int.Parse(celda.ToString());
                        // Determinar a qué jugador pertenece la ficha
                        if (jugador1.FichasSeleccionadas.Exists(f => f.Numero == numeroFicha))
                        {
                            Console.ForegroundColor = jugador1.Color; // Color del jugador 1
                        }
                        else if (jugador2.FichasSeleccionadas.Exists(f => f.Numero == numeroFicha))
                        {
                            Console.ForegroundColor = jugador2.Color; // Color del jugador 2
                        }
                        Console.Write(celda + " ");
                        Console.ResetColor();
                    }
                    else
                    {
                        switch (celda)
                        {
                            case '█':
                                Console.ForegroundColor = ConsoleColor.Blue;
                                Console.Write("██");
                                break;
                            case '◆':
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.Write("◆ ");
                                break;
                            case '⚠':
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.Write("⚠ ");
                                break;
                            default:
                                Console.ForegroundColor = ConsoleColor.White;
                                Console.Write(celda + " ");
                                break;
                        }
                        Console.ResetColor();
                    }
                }
                Console.WriteLine("║");
            }
            Console.WriteLine("╚═══════════════════════════════════════════════════════════════╝");
            Console.ResetColor();
        }
            
            

        public void ActualizarPosicionFicha(int x, int y, char simbolo)
        {
            if (x >= 0 && x < tamaño && y >= 0 && y < tamaño)
            {
                laberinto[x, y] = simbolo;
            }
        }

        public void LimpiarPosicion(int x, int y)
        {
            if (x >= 0 && x < tamaño && y >= 0 && y < tamaño)
            {
                laberinto[x, y] = ' '; // Limpia la posición de la ficha
            }
        }

        public bool EsPosicionValida(int x, int y)
        {
            return x >= 0 && x < tamaño && y >= 0 && y < tamaño && (laberinto[x, y] == ' ' || laberinto[x,y] == '◆' || laberinto[x,y] == '⚠');
        }

        public char GetCell(int x, int y)
        {
            return laberinto[x, y];
        }

        Random random = new Random();
        private void GenerarObjetivos(int cantidad)
        {
            
            objetivosRestantes = cantidad;

            while (cantidad > 0)
            {
                int x = random.Next(1, tamaño - 1);
                int y = random.Next(1, tamaño - 1);

                if (laberinto[x,y] == ' ') // Solo colocar en celdas vacías
                {
                    laberinto[x,y] = '◆'; // Representa un objetivo
                    cantidad--;
                }
            }
        }

        public void RellenarObjetivos()
        {
            if (objetivosRestantes < 10) // Solo generar objetivos si hay menos de 10
            {
                int objetivosFaltantes = 10 - objetivosRestantes; // Calcular cuántos objetivos faltan
                GenerarObjetivos(objetivosFaltantes); // Generar solo los que faltan
            }
        }

        public bool RecolectarObjetivo(int x, int y)
        {
            if (laberinto[x,y] == '◆') // Verifica si hay un objetivo en la posición
            {
                laberinto[x,y] = ' '; // Limpia el objetivo
                objetivosRestantes--;
                return true;
            }
            return false;
        }

        public int ObtenerObjetivosRestantes()
        {
            return objetivosRestantes;
        }

        public void ColocarFichasAleatorias(List<Ficha> fichas)
        {
            Random random = new Random();
       
            foreach (var ficha in fichas)
            {
                while (true)
                {
                    int x = random.Next(1, tamaño - 1);
                    int y = random.Next(1, tamaño - 1);

                    if (laberinto[x,y] == ' ') // Solo colocar en celdas vacías
                    {
                        laberinto[x,y] = Convert.ToChar(ficha.Numero.ToString()); // Coloca la ficha usando su número como carácter
                        break;
                    }
                }
            }
        }
        public void GenerarTrampas(int cantidad)
        {
            Random random = new Random();
            while (cantidad > 0)
            {
                int x = random.Next(1, tamaño - 1);
                int y = random.Next(1, tamaño - 1);
                if (laberinto[x,y] == ' ')
                {
                    laberinto[x,y] = '⚠';
                    cantidad--;
                }
            }
        }

        public bool EsTrampa(int x, int y)
        {
            return laberinto[x, y] == '⚠';
        }   
    }
}