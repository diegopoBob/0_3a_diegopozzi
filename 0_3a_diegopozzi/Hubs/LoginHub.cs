using Microsoft.AspNetCore.SignalR;

namespace _0_3a_diegopozzi.Hubs
{
    public class LoginHub : Hub
    {
        // Mapear email → connectionId
        public static Dictionary<string, string> UsuariosConectados = new();

        public async Task RegistrarEmail(string email)
        {
            UsuariosConectados[email] = Context.ConnectionId;
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var item = UsuariosConectados.FirstOrDefault(x => x.Value == Context.ConnectionId);
            if (!string.IsNullOrEmpty(item.Key))
                UsuariosConectados.Remove(item.Key);

            await base.OnDisconnectedAsync(exception);
        }
    }
} 