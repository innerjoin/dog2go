export class FieldCoordinates {
    FOUR_PlAYERS: FieldCoordinatesData;
    constructor() {
        this.FOUR_PlAYERS = new FieldCoordinatesData(120, [1530, 90, 570, 2010], [90, 570, 2010, 1530]);
    }
}

export class AreaCoordinates {
    x: number;
    y: number;
    xOffset: number;
    yOffset: number;
    xAltOffset: number;
    yAltOffset: number;
    constructor(position: number, coordinates: FieldCoordinatesData) {
        this.x = coordinates.xStart[position];
        this.y = coordinates.yStart[position];
        this.xOffset = coordinates.xOffsetDefinitions[position];
        this.yOffset = coordinates.yOffsetDefinitions[position];
        this.xAltOffset = coordinates.xAlternativeOffsetDefinitions[position];
        this.yAltOffset = coordinates.yAlternativeOffsetDefinitions[position];        
    }
}

export class FieldCoordinatesData {
    cellSpan: number;
    xStart: number[];
    yStart: number[];
    xOffsetDefinitions: number[];
    yOffsetDefinitions: number[];
    xAlternativeOffsetDefinitions: number[];
    yAlternativeOffsetDefinitions: number[];
    constructor(cellSpan: number, xStart: number[], yStart: number[]) {
        this.cellSpan = cellSpan;
        this.xStart = xStart;
        this.yStart = yStart;
        this.xOffsetDefinitions = [-cellSpan, 0, cellSpan, 0];
        this.yOffsetDefinitions = [0, cellSpan, 0, -cellSpan];
        this.xAlternativeOffsetDefinitions = [0, cellSpan, 0, -cellSpan];
        this.yAlternativeOffsetDefinitions = [cellSpan, 0, -cellSpan, 0];
    }
    getAreaCoordinates(position: number): AreaCoordinates {
        return new AreaCoordinates(position, this);
    }
}

//const cellSpan = 40;
//const xStart = [520, 40, 200, 680];
//const yStart = [40, 200, 680, 520];
//const xOffsetDefinitions = [-cellSpan, 0, cellSpan, 0];
//const yOffsetDefinitions = [0, cellSpan, 0, -cellSpan];
//const xAlternativeOffsetDefinitions = [0, cellSpan, 0, -cellSpan];
//const yAlternativeOffsetDefinitions = [cellSpan, 0, -cellSpan, 0];

