class Fraction {
    num: number = 1;
    dem: number = 1;
    
    static asString(fraction: Fraction): string {
        if (fraction.dem === 1) {
            return fraction.num.toString();
        }
        return fraction.num + '/' + fraction.dem;
    }
    
    static fromString(fractionStr: string): Fraction {
        let split: string[] = fractionStr.split("/", 2);
        if (split.length === 1) {
            let value: number = parseInt(split[0]);
            if (!Number.isNaN(value) && value > 0) {
                return {num: value, dem: 1} as Fraction;
            }
        }
        if (split.length === 2) {
            let num: number = parseInt(split[0]);
            let dem: number = parseInt(split[1]);
            if (!Number.isNaN(num) && !Number.isNaN(dem) && num > 0 && dem > 0) {
                return {num: num, dem: dem} as Fraction;
            }
        }
        
        return {num: 1, dem: 1} as Fraction;
    }
    
    static isValid(fractionStr: string): boolean {
        let split: string[] = fractionStr.split("/", 2);
        if (split.length === 1) {
            let value: number = parseInt(split[0]);
            if (!Number.isNaN(value) && value > 0) {
                return true;
            }
        }
        if (split.length === 2) {
            let num: number = parseInt(split[0]);
            let dem: number = parseInt(split[1]);
            if (!Number.isNaN(num) && !Number.isNaN(dem) && num > 0 && dem > 0) {
                return true;
            }
        }
        
        return false;
    }
}
    
export default Fraction;