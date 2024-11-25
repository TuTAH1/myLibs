if (!String.prototype.isNullOrEmpty) {
	String.prototype.isNullOrEmpty = function () {
		return (!this || this.length === 0 || /^\s*$/.test(this));
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

function Tslice(Source, Start, End = null, SliceReturnSourceIfNotFound = sliceReturn.Never, DefaultValueIfNotFound = sliceReturn.Never, LastStart = false, LastEnd = true, IncludeStart = false, IncludeEnd = false, throwException = false) {
	if (Source.isNullOrEmpty())
		if (throwException)
			throw new Error("String is null or empty");
		else 
			return null;

	let start = 0;
	let end = Number.MAX_SAFE_INTEGER;
	let BasicSlice = (typeof Start == "number" || Start.isNullOrEmpty()) && (typeof End == "number" || End.isNullOrEmpty());
	let result = Source;

	if (Start === null || Start === undefined)
		start = 0;
	else
		switch (typeof Start) {
			case "number":
				start = Start;
				if (start < 0) start = Source.length + start; //: count from end if negative
				if (start >= Source.length) return "";
				break;

			case "string":
				start = TindexOf({s: Source.toString(), s2: Start, IndexOfEnd: !IncludeStart //: if IncludeStart, start will be moved to the end of startsWith
							, InvertDirection: LastStart //: if LastStart, search direction will be inverted (right to left)
							}) + (IncludeStart? 0 : 1); //: go out of last letter of s2 if IncludeStart
							
						break;

			case "object":
				if (Start instanceof RegExp) {
					let match = LastStart ? Start.exec(Source) : Start.exec(Source);
					if (!match) return null;
					let result = LastStart ? match[match.length-1] : match[0];
					start = result.index >= 0 ?
						(result.index + (IncludeStart ? 0 : result[0].length)) : 0;
				} else if (Start instanceof Array && Start.every(x => typeof x == "function")) {
					throw new Error("Not implemented");
					//start = Start.length > 0 ?
					//	TindexOf({s: Source, s2: Start, IndexOfEnd: !IncludeStart, RightDirection: !LastStart}) : 0;
					if (start < 0) start = 0;
				} else
					throw new Error("Type of Start is not supported");
				break;

			default:
				throw new Error("Type of Start is not supported");
		}

	 if (start < 0 || start >= Source.length) {
		if (['Always', 'Start'].includes(DefaultValueIfNotFound)) start = 0;
		 else {
			if (['Always', 'Start'].includes(SliceReturnSourceIfNotFound)) return s;
			if (throwException) throw new RangeError('Start index out of range');
			else return null;
		}
	
	}

	if (!BasicSlice)
		result = Source.slice(start);

	if (End === null || End === undefined)
		end = Source.length;
	else
	switch (typeof End) {
		case "number":
			end = End;
			if (end < 0) end = Source.length + end; //: count from end if negative
			if (BasicSlice && start > end) swap(start, end);
			if (end > Source.length) end = Source.length;
			break;
		case "string":
			end = (LastEnd ? Source.lastIndexOf(End) : Source.indexOf(End));
			if (end < 0) end = Source.length;
			if (IncludeEnd) end += End.length;
			break;
		case "object":
			if (End instanceof RegExp) {
				let match = LastEnd ? End.exec(Source).last() : End.exec(Source);
				end = match.index >= 0 ?
					(match.index + (LastEnd ? 0 : match[0].length)) : 0;
			} else if (End instanceof Array && End.every(x => typeof x == "function")) {
				end = End.length > 0 ?
					indexOfT({Source, End, IndexOfEnd: IncludeEnd, RightDirection: !LastEnd}) : 0;
				if (end < 0)
					if (AlwaysReturnString)
						end = Source.length - 1;
					else
						return null;
			} else throw new Error("Type of End is not supported");
			break;
		default:
			throw new Error("Type of End is not supported");
	}

	if (end < 0 || end > Source.length) {
		if (['Always', 'End'].includes(DefaultValueIfNotFound)) end = 0;
		 else {
			if (['Always', 'End'].includes(SliceReturnSourceIfNotFound)) return s;
			if (throwException) throw new RangeError('Start index out of range'); 
			else return null;
		}
	}

	return BasicSlice ?
		result.slice(start, end) :
		result.slice(0, end);
}

function TindexOf(s, s2, start = 0, end = Infinity, invertDirection = false, indexOfEnd = false) {
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

const TitaniumExports = {
    sliceReturn,
    swap,
    Tslice,
    TindexOf
};

// Проверяем среду выполнения и экспортируем соответственно
if (typeof module !== 'undefined' && module.exports) {
    // Node.js environment
    module.exports = TitaniumExports;
} else if (typeof window !== 'undefined') {
    // Browser environment
    window.Titanium = TitaniumExports;
} else {
    // Other environments (e.g. Web Workers)
    self.Titanium = TitaniumExports;
}
