﻿///<reference path="../../Library/JQuery/jqueryui.d.ts"/>

import coords = require("./FieldCoordinates");
import FieldCoordinates = coords.FieldCoordinates;
import FieldCoordinatesData = coords.FieldCoordinatesData;
import AreaCoordinates = coords.AreaCoordinates;
export class GameFieldController {
    private game: Phaser.Game;
    private fieldCoordinates: FieldCoordinatesData;
    private scaleFactor: number;

    private allFields: IMoveDestinationField[] = [];
    private kennelFields: IKennelField[] = [];
    private endFiedls: IEndField[] = [];

    get getKennelFields(): IKennelField[] {
        return this.kennelFields;
    }

    get getAllFields(): IMoveDestinationField[] {
        return this.allFields;
    }



    constructor(game: Phaser.Game, scaleFactor: number) {
        this.scaleFactor = scaleFactor;
        this.game = game;
        

        const fc = new FieldCoordinates(scaleFactor);
        this.fieldCoordinates = fc.fourPlAyerMode;

        this.allFields = [];
    }

    public buildFields = (gameTable: IGameTable) => {
        let currentPos = 0;
        for (let k = 0; k < gameTable.PlayerFieldAreas.length; k++) {
            const area: IPlayerFieldArea = gameTable.PlayerFieldAreas[k];
            let current: IMoveDestinationField = area.Fields[0];
            const areaPos = this.fieldCoordinates.getAreaCoordinates(currentPos);
            // create kennel fields           
            this.addKennelFields(area.KennelFields, areaPos, area.ColorCode);
            let fieldNr = 0;

            const endFields: IEndField[] = this.getEndFields(area.Fields);

            // create destination fields
            while (current) {
                let color = 0xeeeeee;
                if (current.Identifier === area.StartField.Identifier) {
                    const startField: IStartField = <IStartField>current;
                    color = area.ColorCode;
                    let ex = areaPos.x;
                    let ey = areaPos.y;

                    // Generate Endfields from startfield
                    let finalField = startField.EndFieldEntry;
                    for (let j = 0; j < endFields.length; j++) {
                        ex += areaPos.xAltOffset;
                        ey += areaPos.yAltOffset;
                        finalField.viewRepresentation = this.addField(ex, ey, color, finalField.Identifier);
                        this.allFields.push(finalField);

                        finalField = this.getFieldById(finalField.NextIdentifier, area.Fields);
                    }
                }
                current.viewRepresentation = this.addField(areaPos.x, areaPos.y, color, current.Identifier);
                this.allFields.push(current);
                // Calculate Position for next field 
                if (fieldNr < 8 || fieldNr > 11) {
                    areaPos.x += areaPos.xOffset;
                    areaPos.y += areaPos.yOffset;
                } else {
                    areaPos.x += areaPos.xAltOffset;
                    areaPos.y += areaPos.yAltOffset;
                }

                current = this.getFieldById(current.NextIdentifier, area.Fields);
                fieldNr++;
            }
            currentPos++;
        }
    }

    public addKennelFields(kennelFields: IKennelField[], areaPos: AreaCoordinates, color: number) {
        const kennelX = areaPos.x + 11 * areaPos.xOffset;
        const kennelY = areaPos.y + 11 * areaPos.yOffset;
        for (let i = 0; i < kennelFields.length; i++) {
            const kennelField: IKennelField = kennelFields[i];
            let xx = 0;
            let yy = 0;
            switch (i % 4) {
                case 1:
                    xx = areaPos.xOffset;
                    yy = areaPos.yOffset;
                    break;
                case 2:
                    xx = areaPos.xAltOffset;
                    yy = areaPos.yAltOffset;
                    break;
                case 3:
                    xx = areaPos.xOffset + areaPos.xAltOffset;
                    yy = areaPos.yOffset + areaPos.yAltOffset;
                    break;
                default: // do nothing
            }
            kennelField.viewRepresentation = this.addField(kennelX + xx, kennelY + yy, color, kennelField.Identifier);
            this.allFields.push(kennelField);
            this.kennelFields.push(kennelField);
        }
    }

    private addField(x: number, y: number, color: number, id?: number): Phaser.Graphics {
        const graphics = this.game.add.graphics(x, y); // positioning is relative to parent (in this case, to the game world as no parent is defined)
        graphics.beginFill(color, 1);
        graphics.lineStyle(this.scaleFactor * 2, 0x222222, 1);
        graphics.drawCircle(0, 0, this.scaleFactor * 30); //draw a circle relative to it's parent (in this case, the graphics object)
        graphics.endFill();
        return graphics;
    }


    public isValidTargetField(targetField: IMoveDestinationField): boolean {
        if (this.kennelFields.indexOf(targetField) > -1) {
            return false;
        }
        return true;
    }

    private getFieldById(id: number, fields: IMoveDestinationField[]): IMoveDestinationField {
        for (let field of fields) {
            if (id === field.Identifier) {
                return field;
            }
        }
        return null;
    }

    public getFieldByIdOfAll(id: number): IMoveDestinationField {
        for (let field of this.allFields) {
            if (id === field.Identifier) {
                return field;
            }
        }
        return null;
    }

    public getFieldPosition(fieldId: number):Phaser.Point {
        let result: Phaser.Point;
        for (let field of this.allFields) {
            if (field.Identifier === fieldId) {
                result = field.viewRepresentation.position;
                break;
            }
        }
        return result;
    }

    private getEndFields(fields: IMoveDestinationField[]): IEndField[] {
        const result: IEndField[] = [];
        for (let field of fields) {
            if (field.FieldType.localeCompare("dog2go.Backend.Model.EndField") === 0) {
                this.endFiedls.push(field);
                result.push(field);
            }
        }
        return result;
    }
}