function inactiveTimer(dotnetHelper) {
    var timer;
    document.onmousemove = resetTimer;
    document.onkeypress = resetTimer;

    function resetTimer() {
        clearTimeout(timer);
        timer = setTimeout(logout, 1000 * 60 * 15)
    }

    function logout() {
        dotnetHelper.invokeMethodAsync("Logout");
    }
}