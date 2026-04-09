import Fraction from "$lib/models/Fraction.js";
import UnitType from "$lib/models/UnitType.js";

class Amount {
	fraction: Fraction = new Fraction();
	unitType: string = "";

	static asString(amount: Amount): string {
		let result: string = Fraction.asString(amount.fraction) + ' ' + UnitType.asAbbreviation(amount.unitType);
		if (amount.fraction.num == 1 && amount.fraction.dem == 1 && result.endsWith('s')) {
			return result.slice(0, result.length - 1);
		}
		return result;
	}
	
	static isEmpty(amount: Amount): boolean {
		return amount.unitType === "None" || 
			   amount.fraction.num == 0 || 
			  (amount.fraction.num / amount.fraction.dem) < 0.05;
	}
}

export default Amount;