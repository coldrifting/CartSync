class Fraction {
    num: number = 1;
    dem: number = 1;
    
    static asString(fraction: Fraction): string {
        if (fraction.dem === 1) {
            return fraction.num.toString();
        }
        return fraction.num + '/' + fraction.dem;
    }
}
    
export default Fraction;