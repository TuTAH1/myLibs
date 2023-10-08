
String.prototype.isNullOrEmpty = function () {
	return this === null || this === undefined || this === '';
}

function swap(array, aIndex, bIndex) {
	var temp = array[aIndex];
	array[aIndex] = array[bIndex];
	array[bIndex] = temp;
}

String.prototype.slice = function (Start, End, AlwaysReturnString = false, LastStart = false, LastEnd = true, IncludeStart = false, IncludeEnd = false) {
	if (this.isNullOrEmpty())
		if (AlwaysReturnString)
			return null;
		else
			throw new Error("String is null or empty");

	let start;
	let end;
	let BasicSlice = typeof Start == "number" && typeof End == "number";
	let result = this;

	switch (typeof Start) {
		case "number":
			start = Start;
			if (start < 0) start = this.length + start; //: count from end if negative
			if (start < 0 || start >= this.length)
				if (AlwaysReturnString)
					start = 0;
				else
					return null;
			break;
		case "string":
			start = LastStart ? this.lastIndexOf(Start) : this.indexOf(Start);
			if (start < 0) start = 0;
			if (IncludeStart) start += Start.length;
			break;
		case "object":
			if (Start instanceof RegExp) {
				let match = LastStart ? Start.exec(this)[array.length-1] : Start.exec(this)[0];
				start = match.index >= 0 ?
					(match.index + (IncludeStart ? 0 : match[0].length)) : 0;
			} else if (Start instanceof Array && Start.every(x => typeof x == "function")) {
				start = Start.length > 0 ?
					this.indexOfT({Start, IndexOfEnd: !IncludeStart, RightDirection: !LastStart}) : 0;
				if (start < 0) start = 0;
			} else
				throw new Error("Type of Start is not supported");
			break;
		default:
			throw new Error("Type of Start is not supported");
	}

	if (!BasicSlice)
		result = this.slice(start);


	switch (typeof End) {
		case "number":
			end = End;
			if (end < 0) end = this.length + end; //: count from end if negative
			if (BasicSlice && start > end) Swap(start, end);
			if (end > this.length) end = this.length;
			break;
		case "string":
			end = (LastEnd ? this.lastIndexOf(End) : this.indexOf(End));
			if (end < 0) end = this.length;
			if (IncludeEnd) end += End.length;
			break;
		case "object":
			if (End instanceof RegExp) {
				let match = LastEnd ? End.exec(this).last() : End.exec(this);
				end = match.index >= 0 ?
					(match.index + (LastEnd ? 0 : match[0].length)) : 0;
			} else if (End instanceof Array && End.every(x => typeof x == "function")) {
				end = End.length > 0 ?
					this.indexOfT({End, IndexOfEnd: IncludeEnd, RightDirection: !LastEnd}) : 0;
				if (end < 0)
					if (AlwaysReturnString)
						end = this.length - 1;
					else
						return null;
			} else throw new Error("Type of End is not supported");
			break;
		default:
			throw new Error("Type of End is not supported");
	}

	return BasicSlice ?
		result.slice(start, (end - start)) :
		result.slice(0, end);
}


String.prototype.indexOfT = function (Conditions, Start = 0, End = Infinity, RightDirection = true, IndexOfEnd = false) {
	if (End === Infinity) End = this.length - 1;
	if (Start < 0) Start = this.length + Start;
	if (Start < 0) throw new Error(`incorrect negative Start (${Start - this.length}). |Start| should be ≤ this.length (${this.length})`);
	if (End < 0) End = this.length + End;
	if (End < 0) throw new Error(`incorrect negative End (${End - this.length}). |End| should be ≤ this.length (${this.length})`);

	if (End === Start) return -2;

	if (RightDirection && End < Start ||
		!RightDirection && End > Start)
		swap(Start, End);

	let defaultCurMatchPos = RightDirection ? 0 : Conditions.length - 1;
	let curCondition = defaultCurMatchPos;
	let Result = -1;

	if (RightDirection)
		for (let i = Start; i < End; i++) {
			if (Conditions[curCondition](this[i])) {
				curCondition++;
				if (curCondition !== Conditions.length) continue;
				Result = i;
				curCondition = defaultCurMatchPos;
				//if(!LastOccuarance)
				break;
			} else {
				i -= curCondition;
				curCondition = defaultCurMatchPos;
			}
		}
	else
		for (let i = Start; i >= End; i--) {
			if (Conditions[curCondition](this[i])) {
				curCondition--;
				if (curCondition !== 0) continue;
				Result = i;
				curCondition = defaultCurMatchPos;
				//if(!LastOccuarance)
				break;
			} else {
				i += ((Conditions.length - 1) - curCondition);
				curCondition = defaultCurMatchPos;
			}
		}

	return (Result === -1 || IndexOfEnd ^ !RightDirection) ?
		Result : (Result - Conditions.length) + 1;
}
