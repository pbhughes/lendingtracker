(function () {
    console.log("This function runs as soon as the script is loaded!");
    // Add your custom logic here
    // Function to get the initial viewport width
    window.getInitialViewportWidth = () => window.innerWidth;

    // Function to observe viewport size changes and notify Blazor
    window.initializeViewport = (dotNetRef) => {
        // Notify Blazor of the initial viewport width
        dotNetRef.invokeMethodAsync("SetViewportWidth", window.innerWidth);

        // Observe and notify on resize
        const resizeObserver = new ResizeObserver(() => {
            dotNetRef.invokeMethodAsync("SetViewportWidth", window.innerWidth);
        });
        resizeObserver.observe(document.body);
    };
})();


function showYesNoAlert(message) {
    return confim(message);
}

