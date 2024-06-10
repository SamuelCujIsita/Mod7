using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace PeliculasBlazor.Client.Pages
{
    public partial class Counter
    {
        private int currentCount = 0;
        private void IncrementCount()
        {
            currentCount++;
        }
    }
}
