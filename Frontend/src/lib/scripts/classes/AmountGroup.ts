import Amount from "$lib/scripts/classes/Amount.js";

class AmountGroup {
	weight: Amount = new Amount();
	volume: Amount = new Amount();
	count: Amount = new Amount();

	static asString(amounts: AmountGroup): string {
		let result: string[] = [];
		
		if (!Amount.isEmpty(amounts.weight)) {
			result.push(Amount.asString(amounts.weight));
		}
		
		if (!Amount.isEmpty(amounts.volume)) {
			result.push(Amount.asString(amounts.volume));
		}
		
		if (!Amount.isEmpty(amounts.count)) {
			result.push(Amount.asString(amounts.count));
		}
		
		return result.join(', ');
	}
}

export default AmountGroup;