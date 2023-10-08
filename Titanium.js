
//This is reference cs code

// public static string Slice<Ts, Te>(this string s, Ts? Start, Te? End, bool AlwaysReturnString = false, bool LastStart = false, bool LastEnd = true, bool IncludeStart = false, bool IncludeEnd = false)
// {
// 	if (s.IsNullOrEmpty())
// 		if (AlwaysReturnString)
// 			return null;
// 		else
// 			throw new ArgumentNullException(nameof(s));
//
// 	int start;
// 	int end;
// 	bool BasicSlice = Start is int or null && End is int or null;
//
// 	switch (Start)
// 	{
// 		case null:
// 			start = 0;
// 			break;
// 		case int startIndex:
// 			start = startIndex;
// 			if (start < 0) start = s.Length + start; //: count from end if negative
// 			if (start < 0 || start >= s.Length)
// 				if (AlwaysReturnString)
// 					start = 0;
// 				else
// 					return null;
// 			break;
// 		case string startsWith:
// 			start = LastStart ? s.LastIndexOfEnd(startsWith) : s.IndexOfEnd(startsWith);
// 			if (start < 0) start = 0;
// 			if (IncludeStart) start += startsWith.Length;
// 			break;
// 		case Regex startRegex:
// 			var match = LastStart? startRegex.Matches(s).Last() :  startRegex.Match(s);
// 			start = match.Index>=0?
// 				(match.Index + (IncludeStart ? 0 : match.Length)) : 0;
// 			break;
// 		case Func<char,bool>[] startConditions:
// 			start = startConditions?.Any()==true?
// 				s.IndexOfT(startConditions, IndexOfEnd: !IncludeStart, RightDirection: !LastStart) : 0;
// 			if (start < 0) start = 0;
// 			break;
// 		default:
// 			throw new TypeInitializationException(typeof(Ts).FullName, new ArgumentException($"Type of {nameof(Start)} is not supported"));
// 	}
//
// 	if (!BasicSlice) s = s.Slice(start);
//
// 	switch (End)
// 	{
// 		case null:
// 			end = s.Length;
// 			break;
// 		case int endIndex:
// 			end = endIndex;
// 			if (end < 0) end = s.Length + end; //: count from end if negative
// 			if (BasicSlice && start > end) Swap(ref start, ref end);
// 			if (end > s.Length) end = s.Length;
// 			break;
// 		case string endsWith:
// 			end = (LastEnd ? s.LastIndexOf(endsWith) : s.IndexOf(endsWith));
// 			if(end<0) end = s.Length;
// 			if (IncludeEnd) end += endsWith.Length;
// 			break;
// 		case Regex endregex:
// 			var match = LastEnd? endregex.Matches(s).Last() :  endregex.Match(s);
// 			end = match.Index>=0?
// 				(match.Index + (LastEnd ? 0 : match.Length)) : 0;
// 			break;
// 		case Func<char,bool>[] endConditions:
// 			end = endConditions?.Any()!=true?
// 				s.IndexOfT(endConditions,IndexOfEnd: IncludeEnd, RightDirection: !LastEnd) : 0;
// 			if (end < 0)
// 				if (AlwaysReturnString)
// 					end = s.Length-1;
// 				else
// 					return null;
// 			break;
// 		default:
// 			throw new TypeInitializationException(typeof(Ts).FullName, new ArgumentException($"Type of {nameof(End)} is not supported"));
// 	}
//
// 	return BasicSlice ?
// 		s.Substring(start, (end - start)) :
// 		s.Slice(0, end);
// }
//
// /// <summary>
// /// Removes <paramref name="s"/> symbols from 0 to <paramref name="Start"/><para></para>
// /// Supported types: <typeparamref name="int"></typeparamref>, <typeparamref name="string"></typeparamref>, <typeparamref name="Regex"></typeparamref>, <typeparamref name="Func&lt;char,bool&gt;"></typeparamref>;
// /// </summary>
// /// <typeparam name="Ts">Type of the <paramref name="Start"/></typeparam>
// /// <param name="s"></param>
// /// <param name="Start"> Start of the result string <para/>
// ///<list type="table"></list>
// /// /// <item><typeparamref name="default"/>: 0 (don't cut start)</item>
// /// <item><typeparamref name="int"/>: Start index of the result string (inverse direction if negative)</item>
// /// <item><typeparamref name="string"/>: The string inside <paramref name="s"/> that will be the start position of the result</item>
// /// <item><typeparamref name="Regex"/>: The string inside <paramref name="s"/> matches Regex that will be the start position of the result</item>
// /// <item><typeparamref name="Func&amp;lt;char,bool&amp;gt;"/>: Условия, которым должны удовлетворять символы начала строки (по функции на 1 символ)</item>
// /// </param>
//
// /// <param name="AlwaysReturnString">return <paramref name="s"/> if <paramref name="Start"/> or <paramref name="End"/> not found (may be half-cutted)</param>
// /// <param name="LastStart">if true, the last occurance of the <paramref name="Start"/> will be searched <para/> (doesn't do anything if <paramref name="Start"/> is <typeparamref name="int"/>)</param>
// /// <param name="IncludeStart">Include <paramref name="Start"/> symbols <para/> (doesn't do anything if <paramref name="Start"/> is <typeparamref name="int"/>)</param>
//
// /// <returns></returns>
// /// <exception cref=""></exception>
// public static string Slice<Ts>(this string s, Ts? Start, bool AlwaysReturnString = false, bool LastStart = false, bool IncludeStart = false) =>
// s.Slice(Start, int.MaxValue, AlwaysReturnString, LastStart, false, IncludeStart, false);
//
// public static string SliceFromEnd(this string s, string StartsWith = null, string EndsWith = null, bool AlwaysReturnString = false, bool LastStart = false, bool LastEnd = true, bool IncludeStart = false, bool IncludeEnd = false) //:25.08.2022 includeStart, includeEnd
// {
// 	var end = EndsWith==null? s.Length-1 : LastEnd? s.LastIndexOf(EndsWith) : s.IndexOf(EndsWith);
// 	if (end < 0) return  AlwaysReturnString? s : null;
//
// 	s = s.Slice(0, end);
//
// 	var start = StartsWith == null? 0 : LastStart? s.LastIndexOfEnd(StartsWith) : s.IndexOfEnd(StartsWith);
// 	if (start < 0) return  AlwaysReturnString? s : null;
//
// 	return IncludeStart? StartsWith : "" + s.Slice(start) + (IncludeEnd? EndsWith : "");
// }

// public static int IndexOfT(this string s, Func<char,bool>[] Conditions, int Start = 0, int End = Int32.MaxValue, bool RightDirection = true, bool IndexOfEnd = false) //:22.09.22 Bugfix, deleted useless LastOccurance; Replaced DirectionEnum with bool RightDirection
// {
// 	if (End == Int32.MaxValue) End = s.Length-1;
// 	if (Start < 0) Start = s.Length + Start;
// 	if (Start < 0) new ArgumentOutOfRangeException(nameof(Start),Start,$"incorrect negative Start ({Start - s.Length}). |Start| should be ≤ s.Length ({s.Length})");
// 	if (End < 0) End = s.Length + End;
// 	if (End < 0) throw new ArgumentOutOfRangeException(nameof(End),End,$"incorrect negative End ({End - s.Length}). |End| should be ≤ s.Length ({s.Length})");
//
// 	if (End == Start) return -2;
//
// 	if(RightDirection && End < Start ||
// 		!RightDirection && End > Start)
// 		Swap(ref Start, ref End);
//
// 	int defaultCurMatchPos = RightDirection? 0 : Conditions.Length-1;
// 	int curCondition = defaultCurMatchPos;
// 	int Result = -1;
//
// 	if (RightDirection)
// 		for (int i = Start; i < End; i++)
// 	{
// 		if (Conditions[curCondition](s[i]))
// 		{
// 			curCondition++;
// 			if (curCondition != Conditions.Length) continue;
// 			Result = i;
// 			curCondition = defaultCurMatchPos;
// 			//if(!LastOccuarance)
// 			break;
// 		}
// 		else
// 		{
// 			i -= curCondition;
// 			curCondition = defaultCurMatchPos;
// 		}
// 	}
// else
// 	for (int i = Start; i >= End; i--)
// 	{
// 		if (Conditions[curCondition](s[i]))
// 		{
// 			curCondition--;
// 			if (curCondition != 0) continue;
// 			Result = i;
// 			curCondition = defaultCurMatchPos;
// 			//if(!LastOccuarance)
// 			break;
// 		}
// 		else
// 		{
// 			i += ((Conditions.Length-1) - curCondition);
// 			curCondition = defaultCurMatchPos;
// 		}
// 	}
//
// 	return Result = Result == -1 || IndexOfEnd ^ !RightDirection?
// 		Result : (Result - Conditions.Length) +1;
// }

//This is translated to js code:

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
