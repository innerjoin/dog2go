export class FieldCoordinates {
    fourPlAyerMode: FieldCoordinatesData;
    constructor(scaleFactor: number) {
        this.fourPlAyerMode = new FieldCoordinatesData(scaleFactor * 40, [scaleFactor * 510, scaleFactor * 30, scaleFactor * 190, scaleFactor * 670], [scaleFactor * 30, scaleFactor * 190, scaleFactor * 670, scaleFactor * 510]);
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