function drawBarDecorations(canvas) {

    decorationColor = "#000000";

    context = canvas.getContext('2d');

    context.beginPath();
    context.moveTo(0,0);
    context.lineTo(0, canvas.height);
    context.lineTo(canvas.width, canvas.height);
    context.lineTo(canvas.width, 0);
    context.lineTo(0, 0);
    context.closePath();
    context.strokeStyle = decorationColor;
    context.stroke();


    // Create bars at 25%, 50%, and 75%
    context.beginPath();
    context.moveTo((canvas.width * 0.25) * 1, 0);
    context.lineTo((canvas.width * 0.25) * 1, canvas.height);
    context.closePath();
    context.strokeStyle = decorationColor;
    context.stroke();

    context.beginPath();
    context.moveTo((canvas.width * 0.25) * 2, 0);
    context.lineTo((canvas.width * 0.25) * 2, canvas.height);
    context.closePath();
    context.strokeStyle = decorationColor;
    context.stroke();

    context.beginPath();
    context.moveTo((canvas.width * 0.25) * 3, 0);
    context.lineTo((canvas.width * 0.25) * 3, canvas.height);
    context.closePath();
    context.strokeStyle = decorationColor;
    context.stroke();

    context.textBaseline = "middle";
    context.textAlign = "center";
    context.fillStyle = decorationColor;
    context.font = "bold 9pt Arial";
    context.fillText("1", ((canvas.width * 0.25) / 2), (canvas.height / 2) +1);
    context.fillText("2", ((canvas.width * 0.25) / 2) + (canvas.width * 0.25), (canvas.height / 2)+1);
    context.fillText("3", ((canvas.width * 0.25) / 2) + (canvas.width * 0.50), (canvas.height / 2)+1);
    context.fillText("4", ((canvas.width * 0.25) / 2) + (canvas.width * 0.75), (canvas.height / 2) + 1);
}

function drawBarDecorations_Thin(canvas) {

    decorationColor = "#000000";

    context = canvas.getContext('2d');

    
    context.beginPath();
    context.moveTo(0, 0);
    context.lineTo(0, canvas.height);
    context.lineTo(canvas.width, canvas.height);
    context.lineTo(canvas.width, 0);
    context.lineTo(0, 0);
    context.closePath();
    context.strokeStyle = decorationColor;
    context.stroke();
    

    // Create bars at 25%, 50%, and 75%
    context.beginPath();
    context.moveTo((canvas.width * 0.25) * 1, 0);
    context.lineTo((canvas.width * 0.25) * 1, canvas.height);
    context.closePath();
    context.strokeStyle = decorationColor;
    context.stroke();

    context.beginPath();
    context.moveTo((canvas.width * 0.25) * 2, 0);
    context.lineTo((canvas.width * 0.25) * 2, canvas.height);
    context.closePath();
    context.strokeStyle = decorationColor;
    context.stroke();

    context.beginPath();
    context.moveTo((canvas.width * 0.25) * 3, 0);
    context.lineTo((canvas.width * 0.25) * 3, canvas.height);
    context.closePath();
    context.strokeStyle = decorationColor;
    context.stroke();
}


function fillBar(canvas, value) {
    context = canvas.getContext('2d');
    context.fillStyle = "#C0C0C0";
    if ((value >= 0) && (value <= 1)) {             /* Red */
        context.fillStyle = "#FF3300";
    } else if ((value >= 1) && (value <= 2.25)) {  /* Yellow / Orange */
        context.fillStyle = "#FFA500";
    } else if ((value >= 2.25) && (value <= 3.5)) {    /* Green */
        context.fillStyle = "#008000";
    } else if ((value >= 3.5) && (value <= 4)) {  /* Slightly fancier green */
        context.fillStyle = "#008000";
    }

    fillWidth = value * (canvas.width * 0.25);

    
    context.fillRect(0, 0, fillWidth, canvas.height);

    /* Black line at the end of the filled portion */
    /*
    context.beginPath();
    context.moveTo(fillWidth, 0);
    context.lineTo(fillWidth, canvas.height);

    context.closePath();
    context.strokeStyle = "#000000";
    context.stroke();
    */   

}

function addStar(canvas) {
    context = canvas.getContext('2d');
    r = (canvas.height * 0.5);

    x = canvas.width - (r);
    y = (canvas.height / 2);
    
    p = 5;
    m = 0.5;

    context.save();
    context.beginPath();
    context.translate(x, y);
    context.moveTo(0, 0 - r);
    for (var i = 0; i < p; i++) {
        context.rotate(Math.PI / p);
        context.lineTo(0, 0 - (r * m));
        context.rotate(Math.PI / p);
        context.lineTo(0, 0 - r);
    }
    context.fillStyle = "#FFFF00";
    context.fill();
    context.restore();
}

function createOutcomeBarIE(canvasID) {
    decorationColor = "#000000";
    try {
        canvas = document.getElementById(canvasID);
        context = canvas.getContext('2d');

        context.beginPath();
        context.moveTo(0, 0);
        context.lineTo(0, canvas.height);
        context.lineTo(canvas.width, canvas.height);
        context.lineTo(canvas.width, 0);
        context.lineTo(0, 0);
        context.closePath();
        context.strokeStyle = decorationColor;
        context.stroke();

        context.textBaseline = "middle";
        context.textAlign = "center";
        context.fillStyle = decorationColor;
        context.font = "bold 9pt Arial";
        context.fillText("Insufficient Evidence", (canvas.width / 2), (canvas.height / 2) + 1);

    } catch (err) {
        alert('Error: ' + err);
    }
}



function createOutcomeBar(canvasID, value) {
    try {
        canvas = document.getElementById(canvasID);
        context = canvas.getContext('2d');
        fillBar(canvas, value);
        if (value == 4) {
            //addStar(canvas)
        }
        drawBarDecorations(canvas);



    } catch (err) {
        alert('Error: ' + err);
    }

}

function createNumberBar(canvasID) {
    try {
        canvas = document.getElementById(canvasID);
        context = canvas.getContext('2d');
        //fillBar(canvas, value);
        drawBarDecorations(canvas);
    } catch (err) {
        alert('Error: ' + err);
    }
}

function createOutcomeBar_Thin(canvasID, value) {
    try {
        canvas = document.getElementById(canvasID);
        context = canvas.getContext('2d');
        fillBar(canvas, value);        
        drawBarDecorations_Thin(canvas);



    } catch (err) {
        alert('Error: ' + err);
    }

}