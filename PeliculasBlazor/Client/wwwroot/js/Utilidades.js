
function pruebaPuntoNetStatic() {
    DotNet.invokeMethodAsync("PeliculasBlazor.Client", "ObtenerCurrentCount")
        .then(resultado => {
            console.log('conteo desde javascript ' + resultado);
        })
}
