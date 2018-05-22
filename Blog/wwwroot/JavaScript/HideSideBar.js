function hideSideBar(sidebarId) {
    let element = document.getElementById(sidebarId);
    if (element.style.display === 'none') {
        element.style.display = 'block';
    }
    else {
        element.style.display = 'none';
    }
}
