
 
 

 

/// <reference path="Enums.d.ts" />
import Enums = require("Enums");

declare module dog2go.Backend.Model {
    import ColorCode = Enums.ColorCode;

    interface KennelField extends dog2go.Backend.Model.MoveDestinationField {
	}
	interface Meeple {
		ColorCode: Enums.ColorCode;
		CurrentPosition: dog2go.Backend.Model.MoveDestinationField;
	}
	interface MoveDestinationField {
		CurrentMeeple: dog2go.Backend.Model.Meeple;
		Identifier: number;
		Next: dog2go.Backend.Model.MoveDestinationField;
		NextIdentifier: number;
		Previous: dog2go.Backend.Model.MoveDestinationField;
		PreviousIdentifier: number;
	}
	interface Participation {
		Participant: dog2go.Backend.Model.User;
		Partner: dog2go.Backend.Model.User;
	}
	interface PlayerFieldArea {
		ColorCode: ColorCode;
		FieldId: number;
		Fields: dog2go.Backend.Model.MoveDestinationField[];
		Identifier: number;
		KennelFields: dog2go.Backend.Model.KennelField[];
		Meeples: dog2go.Backend.Model.Meeple[];
		Next: dog2go.Backend.Model.PlayerFieldArea;
		NextIdentifier: number;
		Participation: dog2go.Backend.Model.Participation;
		Previous: dog2go.Backend.Model.PlayerFieldArea;
		PreviousIdentifier: number;
	}
	interface User {
		GroupName: string;
		Identifier: number;
		Nickname: string;
	}
}


