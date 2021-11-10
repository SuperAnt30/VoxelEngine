using System;
using System.IO;
using System.Net.Sockets;

namespace VoxelEngine.Network
{
    public class SocketBase : SocketHeir
    {
        /// <summary>
        /// Порт сервера
        /// </summary>
        public int Port { get; protected set; }

        protected SocketBase() { }
        public SocketBase(int port) => Port = port;

        /// <summary>
        /// Метод отправки пакетов запроса
        /// </summary>
        /// <param name="bytes">данные в массиве байт</param>
        /// <returns>результат отправки</returns>
        public bool Sender(Socket socket, byte[] bytes)
        {
            if (!IsConnected || bytes.Length == 0)
            {
                return false;
            }
            try
            {
                // Отправляем пакет
                socket.Send(ReceivingBytes.BytesSender(bytes));
                return true;
            }
            catch (Exception e)
            {
                // Возвращаем ошибку
                OnError(new ErrorEventArgs(e));
                return false;
            }
        }

        /// <summary>
        /// Ответ готовности сообщения
        /// </summary>
        protected void RbReceive(object sender, ServerPacketEventArgs e) => OnReceive(e);

        #region Event

        /// <summary>
        /// Событие, ошибка
        /// </summary>
        public event ErrorEventHandler Error;
        /// <summary>
        /// Событие ошибки
        /// </summary>
        protected void OnError(ErrorEventArgs e) => Error?.Invoke(this, e);

        /// <summary>
        /// Событие, получать
        /// </summary>
        public event ServerPacketEventHandler Receive;
        /// <summary>
        /// Событие получать
        /// </summary>
        protected void OnReceive(ServerPacketEventArgs e) => Receive?.Invoke(this, e);

        #endregion
    }
}
