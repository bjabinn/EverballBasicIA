using Quobject.SocketIoClientDotNet.Client;
using System;
using Newtonsoft.Json;
using System.IO;

namespace EverballDotNet
{
    enum Lado
    {
        derecho = 1,
        izquierdo = 2
    }

    class SocketIoManager
    {
        private Socket _socket;
        private ServerState _serverState;
        private MatchData _matchData;
        private Lado jugandoComo;
        private string _user, _pass, _sala, _passSala;

        public SocketIoManager(string url, string user, string pass, string sala, string passSala)
        {
            _socket = IO.Socket(url);
            _user = user;
            _pass = pass;
            _sala = sala;
            _passSala = passSala;
        }

        public void Conecta()
        {
            _socket.Connect();
        }

        public void Play()
        {
            _socket.On("connect", () =>
            {
                var login = new { name = _user, password = _pass };
                _socket.Emit("login", login);
            });

            _socket.On("server_message", (data) =>
            {
                var dataStr = data as string;
                if (dataStr.IndexOf("Logged in as") == 0)
                {
                    var join = new { name = _sala, password = _passSala };
                    _socket.Emit("join_room", join);
                }
            });

            _socket.On("server_state", (msg) =>
            {                
                var json = JsonConvert.SerializeObject(msg);

                _serverState = JsonConvert.DeserializeObject<ServerState>(json);
                if (jugandoComo == Lado.derecho)
                {
                    for (int i = 0; i < _serverState.Team_1.Length; i++)
                    {
                        Random rnd = new Random();
                        var cap = _serverState.Team_1[i];

                        if (cap.cooldown <= 0)
                        {
                            float diffX = _serverState.Ball.x - cap.x;
                            float diffY = _serverState.Ball.y - cap.y;

                            var angleV = Math.Atan2(diffY, diffX) * 57.2957;
                            var forceV = 1.2f;

                            if (_serverState.Ball.x > cap.x)
                            {
                                forceV = 0.6f;
                                if (_serverState.Ball.y > cap.y)
                                {
                                    angleV = rnd.NextDouble() * 60 + 300;
                                }
                                else
                                {
                                    angleV = rnd.NextDouble() * 60;
                                }
                            }

                            var mov = new CapMovement() { angle = (float)angleV, force = forceV, cap_num = i + 1 };
                            _socket.Emit("client_input", mov);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < _serverState.Team_2.Length; i++)
                    {
                        Random rnd = new Random();
                        var cap = _serverState.Team_2[i];

                        if (cap.cooldown <= 0)
                        {
                            float diffX = _serverState.Ball.x - cap.x;
                            float diffY = _serverState.Ball.y - cap.y;

                            var angleI = Math.Atan2(diffY, diffX) * 57.2957;
                            var forceI = 1.2f;

                            if (_serverState.Ball.x < cap.x)
                            {
                                forceI = 0.6f;
                                if (_serverState.Ball.y > cap.y)
                                {
                                    angleI = rnd.NextDouble() * 60 + 180;
                                }
                                else
                                {
                                    angleI = rnd.NextDouble() * 60 + 90;
                                }
                            }

                            var mov = new CapMovement() { angle = (float)angleI, force=forceI, cap_num= i + 1 };

                            _socket.Emit("client_input", mov);
                        }
                    }
                }
            });

            _socket.On("match_start", (msg) =>
            {
                var json = JsonConvert.SerializeObject(msg);
                _matchData = JsonConvert.DeserializeObject<MatchData>(json);
                jugandoComo = (Lado)(_matchData?.role);
            });

            _socket.On("connect_error", (exception) =>
            {
                var ex = exception as Exception;
            });
        }

        public void Disconnect()
        {
            _socket?.Disconnect();
        }
    } // end class
} //end namespace
