using System;

namespace EverballDotNet
{
    class Program
    {      
        static void Main(string[] args)
        {
            Console.Write("Servidor: ");
            string serverToConnect = Console.ReadLine();
            if (string.IsNullOrEmpty(serverToConnect)) {            
                serverToConnect = "http://code-game.com:3000";
            }
            

            Console.Write("Usuario: ");
            var user = Console.ReadLine();
            if (string.IsNullOrEmpty(user))
            {
                //To be filled
                user = "-----";
            }

            Console.Write("Contraseña: ");
            var pass = Console.ReadLine();
            if (string.IsNullOrEmpty(pass))
            {
                //To be filled
                pass = "-----";
            }

            Console.Write("Sala: ");
            var sala = Console.ReadLine();
            if (string.IsNullOrEmpty(sala))
            {
                //To be filled
                sala = "-----";
            }

            Console.Write("Password de la sala: ");
            var passSala = Console.ReadLine();
            if (string.IsNullOrEmpty(passSala))
            {
                //To be filled
                passSala = "-----";
            }

            SocketIoManager socket = new SocketIoManager(serverToConnect, user, pass, sala, passSala);

            socket.Conecta();
            socket.Play();
            Console.ReadLine();
        } //end Main

    } //end class

} //end namespace






