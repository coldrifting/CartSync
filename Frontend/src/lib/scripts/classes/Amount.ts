import Fraction from "$lib/scripts/classes/Fraction.js";
import UnitType from "$lib/scripts/classes/UnitType.js";

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
}

export default Amount;