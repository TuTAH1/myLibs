// @js-check

if (!String.prototype.isNullOrEmpty) {
	String.prototype.isNullOrEmpty = function () {
	  return (this === null || this === undefined || this === '');
	};
  }

function swap(array, aIndex, bIndex) {
	var temp = array[aIndex];
	array[aIndex] = array[bIndex];
	array[bIndex] = temp;
}

const sliceReturn = Object.freeze({
	Always: 'always',
	Start: 'start',
	End: 'end',
	Never: 'never'
});

String.prototype.Tslice = function (Start, End, SliceReturnSourceIfNotFound = sliceReturn.Never, LastStart = false, LastEnd = true, IncludeStart = false, IncludeEnd = false, throwException = false) {
	if (this.isNullOrEmpty())
		if (throwException)
			throw new Error("String is null or empty");
		else
			return null;

	let start;
	let end;
	let BasicSlice = typeof Start == "number" && typeof End == "number";
	let result = this;

	if (Start === null || Start === undefined)
		start = 0;
	else
		switch (typeof Start) {
			case "number":
				start = Start;
				if (start < 0) start = this.length + start; //: count from end if negative
				if (start >= s.Length) return "";
				break;

			case "string":
				start = indexOfT({s: this, s2: Start, IndexOfEnd: !IncludeStart //: if IncludeStart, start will be moved to the end of startsWith
							, InvertDirection: LastStart //: if LastStart, search direction will be inverted (right to left)
							}) + (IncludeStart? 0 : 1); //: go out of last letter of s2 if IncludeStart
							
						break;

			case "object":
				if (Start instanceof RegExp) {
					let match = LastStart ? Start.exec(this)[array.length-1] : Start.exec(this)[0];
					start = match.index >= 0 ?
						(match.index + (IncludeStart ? 0 : match[0].length)) : 0;
				} else if (Start instanceof Array && Start.every(x => typeof x == "function")) {
					start = Start.length > 0 ?
						indexOfT({s: this, s2: Start, IndexOfEnd: !IncludeStart, RightDirection: !LastStart}) : 0;
					if (start < 0) start = 0;
				} else
					throw new Error("Type of Start is not supported");
				break;

			default:
				throw new Error("Type of Start is not supported");
		}

	 if (start < 0 || start >= this.length) {
        if (['Always', 'Start'].includes(sliceReturnSourceIfNotFound)) return s;
        if (throwException) throw new RangeError('Start index out of range');
        return null;
    }

	if (!BasicSlice)
		result = this.slice(start);

	if (End === null || End === undefined)
		end = this.length;
	else
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


function indexOfT(Conditions, Start = 0, End = Infinity, RightDirection = true, IndexOfEnd = false) {
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

function indexOfT(s, s2, start = 0, end = Infinity, invertDirection = false, indexOfEnd = false) {
    if (end === Infinity) end = s.length - 1;
    if (start < 0) start = s.length + start;
    if (start < 0) throw new RangeError(`Incorrect negative Start (${start - s.length}). |Start| should be ≤ s.Length (${s.length})`);
    if (end < 0) end = s.length + end;
    if (end < 0) throw new RangeError(`Incorrect negative End (${end - s.length}). |End| should be ≤ s.Length (${s.length})`);
    if (s == null) throw new Error('ArgumentNullException: s');
    if (s2 == null) throw new Error('ArgumentNullException: s2');
    if (s2.length === 0) return invertDirection ? s.length - 1 : 0;

    if (end === start) return -2;

    if ((!invertDirection && end < start) || (invertDirection && end > start)) {
        let temp = start;
        start = end;
        end = temp;
    }

    let defaultCurMatchPos = invertDirection ? s2.length - 1 : 0;
    let curMatchPos = defaultCurMatchPos;
    let result = -1;

    if (invertDirection) {
        for (let i = start; i >= end; i--) {
            if (s[i] === s2[curMatchPos]) {
                curMatchPos--;
                if (curMatchPos !== -1) continue;
                result = i;
                break;
            } else {
                i += ((s2.length - 1) - curMatchPos);
                curMatchPos = defaultCurMatchPos;
            }
        }
    } else {
        for (let i = start; i <= end; i++) {
            if (s[i] === s2[curMatchPos]) {
                curMatchPos++;
                if (curMatchPos !== s2.length) continue;
                result = i;
                break;
            } else {
                i -= curMatchPos;
                curMatchPos = defaultCurMatchPos;
            }
        }
    }

    return result = result === -1 || indexOfEnd ^ invertDirection ? result : (result - s2.length) + 1;
}
