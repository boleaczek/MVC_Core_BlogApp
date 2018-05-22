function howLongAgo(date_string) {
    let today = new Date();
    let date = new Date(date_string);
    let timeDiff = Math.abs(today - date);
    let diffDays = Math.ceil(timeDiff / (1000 * 3600 * 24));
    if (diffDays === 1) {
        let diffHrs = Math.ceil(timeDiff / 3600000);
        if (diffHrs === 1) {
            let diffMinutes = Math.ceil(timeDiff / 60000);
            if (diffMinutes == 1) {
                return (`${Math.ceil(timeDiff / 1000)} seconds ago.`);
            }
            console.log(`${diffMinutes} minutes ago.`);
            return `${diffMinutes} minutes ago.`;
        }
        return `${diffHrs} hours ago.`;
    }
    else if (diffDays > 8) {
        return date.toLocaleDateString();
    }
    else {
        return `${diffDays} days ago.`;
    }
}

function getDates(class_name) {
    let elements = document.getElementsByClassName(class_name);
    for (i = 0; i < elements.length; i++) {
        let elementText = elements[i].innerHTML;
        let date = new Date(elementText);
        if (Object.prototype.toString.call(date) === "[object Date]") {
            if (isNaN(date.getTime()) === false) {
                elements[i].innerHTML = howLongAgo(elementText);
            }
            else {
                console.log(`No date found in selected element, ${elementText} found instead.`);
            }
        }
    }
}