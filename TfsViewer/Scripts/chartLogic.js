/**
    \brief: computes the data sets (x, y) pairs
    \param: labels list of labels
    \param: values list of values
    \param: count the lenght of the lists
    \return the newly created data set
*/
function computeDataSet(labels, values, count) {
    var dataSet = new Array();

    for (i = 0; i < count; i++)
        dataSet[i] = { x: labels[i], y: values[i] };

    return dataSet;
}

/**
    \brief: computes the average SP data set
    \param: labels list of labels
    \param: start the starting value
    \param: values list of values
    \param: countSpCompleted the lenght of the lists
*/
function computeDataSetSpAverage(labels, start, value, countSpCompleted) {
    var dataSet = new Array();

    var nextValue = start;
    for (i = 0; i < countSpCompleted - 1; i++) {
        dataSet[i] = { x: labels[i], y: nextValue };
        nextValue = nextValue + value;
    }

    //set the today value
    dataSet[i] = { x: new Date().getTime(), y: nextValue };

    return dataSet;
}

/**
    \brief: computes the prediction
    \param: dataSet the data set
    \param: averageSPValue average SP value
    \param: iterationsNeeded number of needed iterations
    \param: iterationLenght the length of the iteration in days
*/
function computePrediction(dataSet, averageSPValue, iterationsNeeded, iterationLenght) {
    var count = dataSet.length;
    
    //iteration time in ms
    var iterationTime = iterationLenght * 24 * 3600 * 1000;
    
    //next date & next value
    var nextDate = dataSet[count - 1].x + iterationTime;
    var nextValue = dataSet[count - 1].y + averageSPValue;
    
    //auxiliar variable
    var tempIteration = iterationsNeeded;

    while (tempIteration > 0) {
        dataSet.push({ x: nextDate, y: nextValue });
        nextValue = nextValue + averageSPValue;
        nextDate = nextDate + iterationTime;
        tempIteration = tempIteration - 1;
    }
}

/**
    \brief: exteds the total SP chart to match the parameters
    \param: dataSet the data set
    \param: iterationsNeeded iterations needed for a release
    \param: iterationLength the length of an interation in days
*/
function extendTotalSPChart(dataSet, iterationsNeeded, iterationLength) {
    var count = dataSet.length;

    //iteration time in ms
    var iterationTime = iterationLength * 24 * 3600 * 1000;

    //next date & next value
    var nextDate = dataSet[count - 1].x + iterationTime;

    //auxiliar variable
    var tempIteration = iterationsNeeded;

    while (tempIteration > 0) {
        dataSet.push({ x: nextDate, y: dataSet[count - 1].y });
        nextDate = nextDate + iterationTime;
        tempIteration = tempIteration - 1;
    }
}

/**
    \brief: exteds the completed SP chart to match the current day
    \param: dataSet the data set
*/
function extendCompletedSPChart(dataSet) {
    var count = dataSet.length;

    //next date
    var nextDate = new Date().getTime();

    dataSet.push({ x: nextDate, y: dataSet[count - 1].y });
}

/**
    \brief: compute the average values in SP
    \param: start start value
    \param: end end values
    \param: count the number of iterations
*/
function computeSPAverage(start, end, count) {
    var average = (end - start) / count;

    return average;
}

/**
    \brief: parses strings into dates
    \param: strings the strings
*/
function datesFromString(strings) {
    var dates = new Array();
    if (strings.length != null) {
        for (i = 0; i < strings.length; i++) {
            dates[i] = new Date(strings[i]).getTime();
        }
    }

    return dates;
}

/**
    \brief: sets up display data for the chart
    \param: htmlId the id of the contained
    \param: the project name
*/
function setupChart(htmlId, projName) {
    var chart = new CanvasJS.Chart(htmlId, {
        zoomEnabled: true,
        title: {
            text: "BurnUp chart: " + projName,
            fontSize: 20
        }
        ,
        axisX: {
            valueFormatString: "DD-MMM-YYYY",
            labelFontSize: 15
        },
        axisY: {
            title: "Story Points",
            titleFontSize: 20,
            suffix: "SP",
            labelFontSize: 15
        },
        legend: {
            horizontalAlign: "center",
            verticalAlign: "bottom",
            fontSize: 15
        }
    });

    return chart;
}

/**
    \brief: populates the SP chart with data
    \param: chart the chart object
    \param: labels the labels
    \param: valuesSPCompleted the values for the completed SP chart
    \param: valuesSPTotal the values for the total SP chart
    \param: predictionAverageSp the average used for prediction in SP
    \param: iterationsLenght the iteration length in days
*/
function createStoryPointsChart(chart, labels, valuesSPCompleted, valuesSPTotal, predictionAverageSp, iterationLength) {

    var dataSetSpCompleted = computeDataSet(labels, valuesSPCompleted, valuesSPCompleted.length);
    var dataSetSpTotal = computeDataSet(labels, valuesSPTotal, valuesSPTotal.length);

    countSpCompleted = valuesSPCompleted.length;
    countSpTotal = valuesSPTotal.length;

    var average = Math.ceil(computeSPAverage(valuesSPCompleted[0], valuesSPCompleted[countSpCompleted - 1], countSpCompleted - 1));
    var dataSetSpAverage = computeDataSetSpAverage(labels, valuesSPCompleted[0], average, countSpCompleted);
    
    if(predictionAverageSp)
        average = predictionAverageSp;

    var iterationsRemaining = Math.ceil(((valuesSPTotal[countSpTotal - 1] - valuesSPCompleted[countSpCompleted - 1]) / average));
    
    computePrediction(dataSetSpAverage, average, iterationsRemaining, iterationLength);
    extendTotalSPChart(dataSetSpTotal, iterationsRemaining, iterationLength);
    extendCompletedSPChart(dataSetSpCompleted);

    chart.options.data = [
        {
            xValueType: "dateTime",
            name: "Total Story Points",
            showInLegend: true,
            legendMarkerType: "square",
            color: "rgba(25, 25, 25, 0.1)",
            type: "area",
            dataPoints: dataSetSpTotal
        },
        {
            xValueType: "dateTime",
            name: "Completed Story Points",
            showInLegend: true,
            legendMarkerType: "square",
            type: "area",
            color: "rgba(119, 210, 250, 0.7)",
            dataPoints: dataSetSpCompleted
        },
        {
            xValueType: "dateTime",
            name: "average story points",
            showInLegend: true,
            type: "line",
            color: "rgba(0, 0, 0, 0.7)",
            dataPoints: dataSetSpAverage
        }];
}

/**
    \brief: create the percentage chart
    \param: chart the chart object
    \param: labelsPercentage the labels used for this chart
    \param: valuesPercentage the values used for thsi chart
*/
function createPercentageChart(chart, labelsPercentage, valuesPercentage) {
    chart.options.data.push({
        xValueType: "dateTime",
        name: "Percent Unestimated Issues",
        showInLegend: true,
        legendMarkerType: "square",
        type: "stepLine",
        color: "rgb(255, 0, 0)",
        axisYType: "secondary",
        dataPoints: computeDataSet(labelsPercentage, valuesPercentage, labelsPercentage.length)
    });
    chart.options.axisY2 = {
        title: "Percent",
        titleFontSize: 20,
        suffix: "%",
        labelFontSize: 15
    };
}