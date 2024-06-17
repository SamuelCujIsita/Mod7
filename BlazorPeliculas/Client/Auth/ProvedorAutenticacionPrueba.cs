using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace BlazorPeliculas.Client.Auth
{
    public class ProvedorAutenticacionPrueba : AuthenticationStateProvider
    {
        /*
        informar el estado de autenticacion a blazor
        demostrar quien eres 
        se ejecuta inmediatamente el usuario entra en la app
        */
        public async override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            //await Task.Delay(4000);
            //metodo que devuele el estado de autenticacion del usuario
            // Autenticado != Authorizado
            var UsuarioYo = new ClaimsIdentity(
                new List<Claim>
                {
                    new Claim(ClaimTypes.Name, "Felipe"),
                    new Claim("edad", "18"),
                    new Claim("llave", "01011"),
                    new Claim(ClaimTypes.Role, "admin")
                }, authenticationType: "Prueba"
                );//USUARIO YOOOOOO


            var anonimo = new ClaimsIdentity(new ClaimsIdentity()); //USUARIO ANONIMO
            return await Task.FromResult(new AuthenticationState(new ClaimsPrincipal(UsuarioYo)));
        }
    }
}
