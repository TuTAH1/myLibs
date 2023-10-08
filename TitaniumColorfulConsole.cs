using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using System.Text;
using System.Threading;
using Colorful;
using Titanium;
using TitaniumColorful;
using ЛБ_2;
using static Colorful.Console;
using Console = Colorful.Console;

/*
 	public static class Colors {
		public static Color 
			White = Color.White.Str().ToColor()        //1
			, Gray = Color.Gray.Str().ToColor()            //2
			, Black = Color.Black.Str().ToColor()      //3
			, Lime = Color.YellowGreen.Str().ToColor() //4
			, Red = Color.FromArgb(255, 30, 60)    //5
			, Orange = Color.FromArgb(255, 165, 0) //6
			, Select = Color.FromArgb(0, 80, 80) //7
			, WeakSelect = Color.FromArgb(0, 40, 40) //8
			, SeaGreen = Color.SeaGreen.Str().ToColor()   //9
			, Yellow = Color.Yellow.Str().ToColor()        //10
			, Cyan = Color.Cyan.Str().ToColor()            //11
			, Violet = Color.BlueViolet.Str().ToColor() //12
			, Silver = Color.Silver.Str().ToColor()        //13
			, Green = Color.FromArgb(73,110,0) //14
			, DefText = White
			, DefBgr = Black
			;
	}
	*/

namespace TitaniumColorful {
	public static class Matrix
	{
		public class WriteOptions //TODO: Добавить TextColorMassage
		{
			public string[] INames;
			public string[] JNames;
			public string TextColorHeaders;
			public string TextColorError;
			public string TextColorTip;
			public string TextColorMessage;
			public string TextColor;
			public string TextColorActive;
			public string TextColorActiveHeader;
			public string TextColorDisabled;
			public string TextColorInfinity;
			public string TextColorZero;
			public string BgrColor;
			public string BgrColorActive;
			public string BgrColorActiveHeader;
			public string BgrColorDisabled;
			public string BgrColorInfinity;
			public string BgrColorZero;
			public bool DisableDiagEdit;
			public double DefaultValue;

			/// <summary>
			/// 
			/// </summary>
			/// <param name="jNames">Имена строк, должно совпадать с количеством строк в матрице или == 1 (тогда это имя применится ко всем столбцам); "%#" подменяется номером строк</param>
			/// <param name="iNames">Имена столбцов, аналогично</param>
			/// <param name="textColorHeaders"> Цвет имён строк и столбцов </param>
			/// <param name="textColor"> Цвет текста</param>
			/// <param name="textColorActive">Цвет текста выделенной ячейки</param>
			/// <param name="BgrColorActive">Цвет заливки выделенной ячейки</param>
			public WriteOptions(
				string[] iNames = null, 
				string[] jNames = null, 
				bool disableDiagEdit = false, 
				double defaultValue = 0,
				string textColor = "default",
				string bgrColor = "default",
				string textColorHeaders = "DarkCyan",
				string textColorActiveHeader = "Orange",
				string textColorActive = "Black", 
				string bgrColorActive = "Cyan",
				string bgrColorActiveRow = "DarkCyan",
				string textColorZero = "Silver", 
				string bgrColorZero = "null",
				string textColorInfinity = "BlueViolet",
				string bgrColorInfinity = "null",
				string textColorDisabled = "Gray",
				string bgrColorDisabled = "null",
				string textColorError = "Red",
				string textColorTip= "Gray",
				string textColorMessage = "Yellow"
				)
			{
				INames=iNames;
				JNames=jNames;
				DisableDiagEdit = disableDiagEdit;
				DefaultValue = defaultValue;

				TextColor=textColor;
				BgrColor = bgrColor;

				TextColorHeaders=textColorHeaders;
				TextColorActive=textColorActive;
				BgrColorActive = (bgrColorActive == "default")?			bgrColor : bgrColorActive;
				TextColorActiveHeader = textColorActiveHeader;

				BgrColorActiveHeader = (bgrColorActiveRow == "default")?	bgrColor : bgrColorActiveRow;

				TextColorDisabled = (textColorDisabled == "default")?	textColor : textColorDisabled;
				BgrColorDisabled = (bgrColorDisabled == "default")?		bgrColor : bgrColorDisabled;

				TextColorInfinity = (textColorInfinity == "default")?	bgrColor : textColorInfinity;
				BgrColorInfinity = (bgrColorInfinity == "default")?		bgrColor : bgrColorInfinity;

				TextColorZero = textColorZero;
				BgrColorZero = bgrColorZero;

				TextColorError = textColorError;
				TextColorTip = textColorTip;
				TextColorMessage = textColorMessage;

				DefaultValue = defaultValue;

			}
		}
		/// <summary>
		/// Выводит матрицу в консоли
		/// </summary>
		/// <param name="matrix">Двумерная матрица [столбцы, строки]</param>
		/// <param name="iActive">Номер подсвеченного столбца</param>
		/// <param name="jActive"> Номер подсвеченной строки (если указаны оба, будет подсвечена ячейка)</param>
		public static void Print(this string[,] matrix, int iActive = -1, int jActive = -1, WriteOptions WO = null)  //:09.10.2021 bugfix
		{
			Console.OutputEncoding = Encoding.UTF8;
			string DefColor = "default", DefBgr = "null";//(Consol.DefaultTextColor!=wo.TextColor)? Consol.DefaultTextColor ://null(Consol.DefaultBackgroundColor != wo.BgrColor)? Consol.DefaultBackgroundColor : ;
			if (Consol.DefaultTextColor != WO.TextColor&&WO.TextColor!="default")// if def color has been changed
			{
				DefColor = WO.TextColor;
				Consol.RClr(WO.TextColor);
			}

			if (WO == null) WO = new WriteOptions();
			bool SameJNames = false, SameINames = false;

			if (WO.INames==null&&WO.JNames==null) //Если имена строк и столбцов не назначены, назначить пустую строку
			{
				WO.JNames = new []{""};
				WO.INames = new []{""};
				SameJNames=SameJNames=true;
			}
			else if (
				WO.INames.Length>1&&WO.JNames.Length>1&&
				WO.JNames.Length!=matrix.GetLength(0)&&
				WO.INames.Length!=matrix.GetLength(1)
			)
				throw new ArgumentException($"Количество имён строк/столбцов ({WO.INames.Length}/{WO.JNames.Length}) не совпадает с количеством строк/столбцов ({matrix.GetLength(1)}/{matrix.GetLength(0)})");

			if (WO.INames.Length==1) SameINames=true;
			if (WO.JNames.Length==1) SameJNames=true;

			int J = matrix.GetLength(0),
				I = matrix.GetLength(1);


			string[] newINames = new string[I];
			string[] newJNames = new string[J];

			for (int i = 0; i<I; i++) //замена "%#" на номер
			{
				int INi = SameINames ? 0 : i; //если имена столбцов одинаковы, то берется первое (единственное) имя
				newINames[i]=WO.INames[INi].Replace("%i", i.ToString());
			}

			int MaxJNameLength = 0;
			for (int j = 0; j<J; j++)
			{
				int INj = SameJNames ? 0 : j;
				newJNames[j]=WO.JNames[INj].Replace("%i", j.ToString()).Replace("%j", j.ToString());
				MaxJNameLength = Math.Max(newJNames[j].Length, MaxJNameLength);
			}

			int[] MaxValLength = new int[I];
			for (int i = 0; i < I; i++)         // Нахождение самой длинной строки в столбце
			{
				int max = 0;
				for (int j = 0; j < J; j++)
				{
					if (matrix[j, i] == double.MaxValue.ToString()) matrix[j, i] = "∞";
					if (matrix[j, i].Length>max)
						max=matrix[j, i].Length;
				}

				MaxValLength[i] = Math.Max(max, newINames[i].Length);
			}

			Write(new string(' ', MaxJNameLength)+ " ");

			for (int i = 0; i < I; i++)
			{
				Consol.Clr(WO.TextColorHeaders,WO.BgrColor);
				if (i==iActive) Consol.Clr(WO.TextColorActiveHeader,WO.BgrColorActiveHeader);
				Write(newINames[i].FormatString(MaxValLength[i], TypesFuncs.Positon.center) + " "); //Вывод названий столбцов
				if (i==I-1) Consol.ReWrite(BackgroundColor: WO.BgrColor);
			}

			for (int j = 0; j<J; j++) //Цикл вывода матрицы
			{
				Consol.Clr(WO.TextColorHeaders,WO.BgrColor);
				if (j==jActive) Consol.Clr(WO.TextColorActiveHeader, WO.BgrColorActiveHeader);
				Write("\n");
				Write(newJNames[j].FormatString(MaxJNameLength, TypesFuncs.Positon.center) + " "); //Вывод названия строки
				if (jActive >= 0)
				{
					if (iActive < 0) //Если активного столбца не выбрано
						if (jActive == j) Consol.Clr(WO.TextColorActive, WO.BgrColorActive); //Если текущая строка – активная, окрас текста
						else Consol.Clr(WO.TextColor,WO.BgrColor);
				}

				for (int i = 0; i<I; i++)
				{
					Consol.RClr();
					if (WO.DisableDiagEdit && i==j) Consol.Clr(WO.TextColorDisabled, WO.BgrColorDisabled); else Consol.Clr(WO.TextColor, WO.BgrColor);

					if (iActive >= 0)
					{
						if (jActive < 0)
						{ if (iActive == i) Consol.Clr(WO.TextColorActive, WO.BgrColorActive); }
						else if (iActive == i && jActive == j) Consol.Clr(WO.TextColorActive, WO.BgrColorActive);
					}

					if (matrix[j, i] == "∞") Consol.Clr(WO.TextColorInfinity, WO.BgrColorInfinity);
					if (matrix[j,i]=="0") Consol.Clr(WO.TextColorZero, WO.BgrColorZero);
						Write(matrix[j, i].FormatString(MaxValLength[i], TypesFuncs.Positon.center) + " ");
				}
				Consol.ReWrite(BackgroundColor: WO.BgrColor);
			}
		}
		public static int LongestString(string[,] matrix)
		{
			int max = 0;
			foreach (string el in matrix)
			{
				if (el.Length > max)
					max = el.Length;
			}

			return max;
		}

		public class ReadOptions
		{
			public bool Symmetrix;
			public string Message;
			public bool ControlMassage;
			public WriteOptions WO;
			/// <summary>
			/// 
			/// </summary>
			/// <param name="defaultValue">Значение, на которое заменяются все записи, не соответствующие формауту double</param>
			/// <param name="symmetrix">Симметричная ли матрица</param>
			/// <param name="DisableDiagEdit">Отключить ли редактирвоание диагонали</param>
			/// <param name="message">Сообщение, показывающееся во время ввода матрицы</param>
			/// <param name="controlMassage">Показывать ли управление</param>
			/// <param name="wo">Write Options</param>
			public ReadOptions(bool symmetrix = false, string message = "Введите матрицу", bool controlMassage = true, WriteOptions wo = null)
			{
				Symmetrix = symmetrix;
				Message = message;
				ControlMassage = controlMassage;
				WO = wo?? new WriteOptions();
			}
		}

		public static double[,] Read(string[,] matrix = null, ReadOptions RO = null)
		{
			int curT = CursorTop;
			bool CurVi = CursorVisible;
			CursorVisible = false;
			int i = 0, j = 0,
				I = matrix.GetLength(1),
				J = matrix.GetLength(0);
			bool Exit = false, ignoreInput = false;

			RO ??= new ReadOptions();
			Console.CursorVisible = false;
			ConsoleKeyInfo CurrentKey = new ConsoleKeyInfo();
			do
			{
				string curStr = matrix[j, i];
				Consol.ReWrite(RO.Message
					               .Replace("%i", RO.WO.INames.Length==matrix.GetLength(1)? RO.WO.INames[i] : i.ToString())
					               .Replace("%j", RO.WO.JNames.Length==matrix.GetLength(0)? RO.WO.JNames[j] : j.ToString())
				               +"\n",ClearLine:true,CurPosH:Consol.CPH.Left,TextColor:RO.WO.TextColorMessage); //BUG: Почему-то не работает ClearLine TODO:
				Consol.ReWrite("Используйте стрелки, чтобы перемещаться по матрице, enter чтобы завершить ввод",CurPosH: Consol.CPH.Left,CurPosV: Consol.CPV.Bottom, TextColor: RO.WO.TextColorTip);
				matrix.Print(i, j, RO.WO);
				if (ignoreInput) ignoreInput = false;
				else CurrentKey = ReadKey(true);
				Console.BufferHeight = Console.WindowHeight+3;
				switch (CurrentKey.Key)
				{
					case ConsoleKey.DownArrow:
						if (j == J - 1)
						{
							if (i == I - 1) { i = 0; j = 0; } //последняя ячейка
							else { j = 0; i++; } //последняя строка
						}
						else j++;
						break;

					case ConsoleKey.UpArrow:
						if (j == 0) {
							if (i == 0) { i = I-1; j = J-1; } //перавая ячейка
							else { i--; j=J-1; } //первая строка
						}
						else j--;
						break;

					case ConsoleKey.Spacebar:
					case ConsoleKey.Tab:
					case ConsoleKey.RightArrow:
						if (i==I-1) {
							if (j==J-1) { i = 0; j = 0; } //последняя ячейка
							else { j++; i=0; } //последний столбец
						} else i++;
					break;

					case ConsoleKey.LeftArrow:
						if (i==0) {
							if (j==0) { j = J-1; i = I-1; } //первая ячейка
							else { j--; i=I-1; } //первый столбец
						} else i--;
					break;

					case ConsoleKey.Enter:
						Consol.Clr(RO.WO.TextColorError,RO.WO.BgrColor); Write("\nВы уверены, что хотите выйти? Нажмите Enter ещё раз, чтобы подтвердить"); Consol.Clr();
						CurrentKey = ReadKey();
						if (CurrentKey.Key == ConsoleKey.Enter)
						{
							Exit = true;
						}
						else ignoreInput = true;
					break;

					default:
						{
							if (RO.WO.DisableDiagEdit)
								if (i == j) break;

							if (CurrentKey.KeyChar.IsDoubleT()) //TODO: добавить поддержку отрицательных чисел
							{
								if (curStr == "" && CurrentKey.KeyChar.ToString()==CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator)
									curStr = "0";
								curStr += CurrentKey.KeyChar;
							}
							else if (CurrentKey.Key==ConsoleKey.Backspace)
							{
								if (curStr.Length>0) curStr = curStr.Substring(0, curStr.Length - 1);
							}

							updMatrix(matrix,curStr,j,i,RO.Symmetrix);
						}
						break;
				}
				Consol.ClearWindow();
				CursorTop = curT;
				CursorLeft = 0;
			} while (!Exit);
			Console.CursorVisible = true;
			double[,] dmatrix = new double[J, I];
			for (j = 0; j < J; j++)
			{
				for (i = 0; i < I; i++)
				{
					if (!double.TryParse(matrix[j, i], out dmatrix[j, i])) dmatrix[j,i] = RO.WO.DefaultValue; //если парситься, засунуть это значение в dmatrix, иначе засунуть defaultValue;
				}
			}

			CursorVisible = CurVi;
			return dmatrix;
		}

		private static void updMatrix(string [,] matrix, string curStr, int j, int i, bool Symmetrix)
		{
				matrix[j, i] = curStr;
				if (Symmetrix) matrix[i, j] = curStr;
		}

		/// <summary>
		/// Создаёт матрицу
		/// </summary>
		public class Create
		{
			public static string[,] FillDiag(int Columns, int Rows = 0, string defaultStr = "", string diagStr = "x")
			{
				Rows=(Rows==0) ? Columns : Rows; //если m не назначен, m=n
				string[,] mx = new String[Rows, Columns];

				for (int j = 0; j<mx.GetLength(0); j++)
				{
					for (int i = 0; i < mx.GetLength(1); i++)
					{
						mx[j,i] = (i == j) ? diagStr : defaultStr;
					}
				}

				return mx;
			}

			public static string[,] FillAll(int Columns, int Rows = 0, string defaultStr = "")
			{
				Rows=(Rows==0) ? Columns : Rows; //если m не назначен, m=n
				string[,] mx = new string[Rows,Columns];

				for (int j = 0; j<mx.GetLength(0); j++)
				{
					for (int i = 0; i < mx.GetLength(1); i++)
					{
						mx[j,i] = defaultStr;
					}
				}

				return mx;
			}
		}
	}

	public static class Consol
	{
		#region Color
		public static string DefaultTextColor { get; private set; } = Color.White.Str();
		public static string DefaultBackgroundColor { get; private set; } = Color.Black.Str();

		public static Color ToColor(this string str)
		{
			string[] RGBstr = str.Split(',');
			if (RGBstr.Length != 3)
			{
				if (str.Length < 2)
					throw new ArgumentException($"Color name should have at least 2 symbols. This have {str.Length}");
				str = char.ToUpper(str[0]) + str[1..];
				Color Debug = Color.FromName(str);
				return Debug;
			}
				
			int[] RGB = new int[3];
			for (int i = 0; i < 3; i++)
			{
				RGB[i] = RGBstr[i].ToIntT();
				if (RGB[i]<0||RGB[i]>255) throw new ArgumentOutOfRangeException($"{i}th parametr is out of range (0-255)");
			}

			return Color.FromArgb(RGB[0], RGB[1], RGB[2]);
		}

		public static string Str(this Color color)
		{
			return $"{color.R}, {color.G}, {color.B}";
		}

		/// <summary>
		/// Changes text and background color. "null" = don't change
		/// Изменяет цвет текста и фона. "null" = не изменять
		/// </summary>
		/// <param name="textColor"></param>
		/// <param name="backgroundColor"></param>
		public static void Clr(string TextColor = "default", string BackgroundColor = "null")
		{
			TextColor??="null";
			BackgroundColor ??= "null";
			if (TextColor == "") TextColor = "default";
			if (BackgroundColor == "") BackgroundColor = "default";

			if (TextColor.ToLower() != "null")
				ForegroundColor = (TextColor.ToLower() == "default")?
					DefaultTextColor.ToColor() : TextColor.ToColor();

			if (BackgroundColor.ToLower() != "null")
				Console.BackgroundColor = (BackgroundColor.ToLower() == "default")?
					DefaultBackgroundColor.ToColor() : BackgroundColor.ToColor();
		}

		public static void BClr(string BackgroundColor = "default")
		{
			Clr("null",BackgroundColor);
		}

		/// <summary>
		/// Изменяет и применяет цвет текста и фона по умолчанию
		/// </summary>
		/// <param name="newDefTextClr">new Default Text Color;
		/// "null" – не изменять цвет текста, "default" – применить дефолтный цвет текста, иначе – Изменить цвет текста по умолчанию</param>
		/// <param name="newDefBgrClr">new Default Background Color;
		/// "null" – не изменять цвет фона, "default" – применить дефолтный цвет фона, иначе – Изменить цвет фона по умолчанию</param>
		public static void RClr(string newDefTextClr = "default", string newDefBgrClr = "default")
		{
			newDefTextClr??="null";
			newDefBgrClr ??= "null";
			newDefBgrClr.ToLower();
			newDefTextClr.ToLower();
			if (newDefTextClr != "default" && newDefTextClr != "null") DefaultTextColor = newDefTextClr;
			if (newDefTextClr !="null") ForegroundColor = DefaultTextColor.ToColor();

			if (newDefTextClr != "default" && newDefTextClr !="null") DefaultBackgroundColor = newDefBgrClr;
			if (newDefBgrClr !="null") BackgroundColor = DefaultBackgroundColor.ToColor();
		}

		#endregion

		#region Input
		public static bool qu(string q = "", string TextColor = "Orange")
		{
			Console.Write(" ");
			bool answer;
			ChangeRead(ref q,TextColor:TextColor);

			if ((q[0] == 'д' || q[0] == 'y' || q[0] == '1' || q.Contains("конечно") || q.Contains("Sbrakets") || q.Contains("ок") || q.Contains("ладно") || q.Contains("угу") || q.Contains("ага") || q.Contains("хорошо") || q.Contains("хочу"))&&!q.Contains("нет")) answer = true;
			else answer = false;
			//Color Red = Color.FromArgb(255, 30, 60), Lime = Color.YellowGreen.Str().ToColor();
			ReWrite(q,ShiftRight:q.Length,TextColor: answer? ЛБ_2.Colors.Lime.Str():  ЛБ_2.Colors.Red.Str());

			return answer;
		}

		public static bool quSwitch(string DefaultValue = "Нет", string TrueTextColor = "Lime", string FalseTextColor = "Red", string BackgroundColor = "Select")
		{
			if (FalseTextColor == "Red") FalseTextColor = Colors.Red.Str();
			if (TrueTextColor == "Lime") TrueTextColor = Colors.Lime.Str();
			if (BackgroundColor == "Select") BackgroundColor = Colors.Select.Str();
			bool OldCursorVisible = CursorVisible;
			CursorVisible = false;

			string oldColor = Console.ForegroundColor.Str();
			string oldBColor = Console.BackgroundColor.Str();
			ConsoleKey key;
			bool result = DefaultValue.ToBool();
			BClr(BackgroundColor);
			Console.Write(result? "← Да ":"Нет →", result? TrueTextColor.ToColor():FalseTextColor.ToColor());
			do
			{
				key = ReadKey(true).Key;
				switch (key)
				{
					case ConsoleKey.LeftArrow:
						if (DefaultValue.ToBool() == true)
						{
							DefaultValue = "Нет";
							ReWrite("Нет →", 0, -5, TextColor:FalseTextColor);
							result = false;
						}

						break;

					case ConsoleKey.RightArrow:
						if (DefaultValue.ToBool() == false)
						{
							DefaultValue = "Да";
							ReWrite("← Да ", 0, -5, TextColor:TrueTextColor);
							result = true;
						}

						break;
				}
			} while (key!=ConsoleKey.Enter&&key!=ConsoleKey.Escape);

			CursorLeft = 0;
			CursorTop += 1;
			CursorVisible = OldCursorVisible;
			Clr(oldColor,oldBColor);
			return result;
		}

		/// <summary>
		/// Функция вопроса с несколькими вариантами ответа
		/// </summary>
		/// <returns>0 = нет; 1 = да; 2 = не знаю</returns>
		public static byte quB(string TextColor = "Orange")
		{
			Console.Write(" ");
			string q; byte answer = 0;
			Clr(TextColor);
			q = ReadLine();
			Clr();

			if (q[0] == 'д' || q[0] == 'y' || q[0] == '1' || q.Contains("конечно") || q.Contains("Sbrakets") || q.Contains("ок") || q.Contains("ладно") || q.Contains("угу") || q.Contains("ага") || q.Contains("хорошо") || q.Contains("хочу")) answer = 1; // да
			if (q.Contains("что") || q.Contains("как") || q.Contains("зачем") || q.Contains("почему") || q.Contains("понима")) answer = 2; // вопрос непонятен
			if (q.Contains("нет")) answer = 0; //нет

			return answer;
		}

		public class ReadOutput //:08.11.2021
		{
			private string _String;
			//private uint UInt;
			public ConsoleKey KeyPressed;
			public ReadKeyType KeyType;

			public ReadOutput( ConsoleKey keyPressed, ReadKeyType keyType, string s = null)
			{
				_String = s;
				KeyPressed = keyPressed;
				KeyType = keyType;
			}

			public double Double() => _String.ToDoubleT();

			public long Long() => _String.ToLongT();
			

			public int Int() => _String.ToIntT();
			public BigInteger BigInteger() => System.Numerics.BigInteger.Parse(_String);

			public string String() => _String;
		}

		public enum ReadKeyType
		{
			StopKey,
			CancelKey
		}

		/// <summary>
		/// Читает и выводит на экран только клавиши, соответствующие указанному типу, остальные игнорирует
		/// </summary>
		/// <param name="InputType">Вводимы тип</param>
		/// <param name="StopReadKeys">Список клавиш, по нажатию которых ввод завершается. По умолчанию – Enter</param>
		/// <param name="ThrewStopKeys">Помещает код последней нажатой клавиши (завершения ввода) в 3 последние символы строки</param>
		/// <param name="Print">Выводить на экран вводимый текст</param>
		/// <param name="InputString">Изначальная вводимая строка (по умолчанию, пуста)</param>
		/// <param name="ShowError">Показывать ли ошибку при нажатии неверной клавиши</param>
		/// <param name="ErrorTextColor">Цвет текста ошибки</param>
		/// <param name="ErrorPosition">вертикальная позиция (отступ) строки, где будет выведена ошибка. Исключительные значения: Int32.Maxvalue – в самом низу окна, Int32.MinValue – в самом верху окна</param>
		/// <param name="isErrorPosition_Absolute">Является ли ErrorPosition абсолютным значением (иначе, это отступ от текущей строки). Игнорируется при исключительных значениях</param>
		/// <returns></returns>
	public static ReadOutput ReadT(Input InputType = Input.String, ConsoleKey[] StopReadKeys = null, ConsoleKey[] CancelReadKeys = null, bool Print = true, string TextColor = "Orange", string InputString = "", bool ClearLine = false, bool ShowError = false, string ErrorTextColor = null, int ErrorPosition = int.MaxValue, bool isErrorPosition_Absolute = false, int? MaxSymbols = null, BigInteger? MaxValue = null, BigInteger? MinValue = null) //:08.11.2021
		{	
			//TODO: Сделать изменение цвета текста после завершения ввода
			//TODO: Сделать показ ошибки при нажатии неподдерживаемой клавиши: добавить  bool ShowError = false, string ErrorTextColor = "Red", int ErrorPosition = Int32.MaxValue, bool isErrorPosition_Absolute = false
			//var curColor = ForegroundColor.Str();

			ConsoleKey ro_keyPressed = default;
			ReadKeyType ro_keyType = default;
			Clr(TextColor);
			if (ClearLine) ReWrite();
			ConsoleKeyInfo CurrentKey;
			bool Stop, Read, isErrorPrinted = false;
			string SingleUseKeys = "";
			bool[] SUKused;
			int SUKcount = 0;
			StopReadKeys ??= new[] { ConsoleKey.Enter }; //TODO:Вообще, лучше создать отдельный класс ReturnKeys, содержащий массив ConsoleKey и переменную ConsoleKeyType (возможно, в виде строки или числа), а в функцию сувать этот массив класса ReturnKeys
			CancelReadKeys ??= new[] { ConsoleKey.Escape };
			ErrorTextColor ??= Colors.Red.Str();

			ReadT_PrintErrorParametrs p = new ReadT_PrintErrorParametrs(ShowError, ErrorTextColor, ErrorPosition, isErrorPosition_Absolute);
			static void Error(string Text, ReadT_PrintErrorParametrs p, out bool isErrorPrinted)
			{
				if (p.ShowError)
				{
					CPV cpv = p.ErrorPosition == int.MaxValue ? CPV.Bottom : p.ErrorPosition == int.MinValue ? CPV.Up : CPV.None;
					int LinesUp = cpv == CPV.None? p.ErrorPosition : 0;
					if (p.isErrorPosition_Absolute) cpv = CPV.Up;

					Console.Write('\a'); //: Beep
					ReWrite(Text,LinesUp,CurPosV:cpv, TextColor: p.ErrorTextColor,CurPosH:CPH.Left,RestoreCurPos:true,ClearLine:true);
					isErrorPrinted = true;
				}
				else
				{
					isErrorPrinted = false;
				}
			}
			 

			/*switch (InputType)
			{
				case Input.Int: 
					SingleUseKeys += '-';
					MaxValue = int.MaxValue;
					MinValue = int.MinValue;
					break;

				case Input.UInt:
					MinValue = uint.MinValue;
					MaxValue = uint.MaxValue;
					break;

				case Input.Long:
					MinValue = long.MinValue;
					MaxValue = long.MaxValue;
				default:
					break;
			}*/

			if (InputType == Input.Double || InputType == Input.UDouble)
			{
				SingleUseKeys += ',';
			}

			if (InputType == Input.Int || InputType == Input.Double || InputType == Input.Long || InputType == Input.BigInteger)
			{
				SingleUseKeys += '-';
			}

			MaxValue ??= InputType == Input.UInt ? uint.MaxValue : InputType == Input.Int ? int.MaxValue : InputType == Input.Long ? long.MaxValue : InputType == Input.BigInteger ? null : -MinValue;
			MinValue ??= InputType == Input.UInt ? uint.MinValue : InputType == Input.Int ? int.MinValue : InputType == Input.Long ? long.MinValue : InputType == Input.BigInteger? null: -MaxValue;

			if (MaxValue == null || MinValue == null) MaxSymbols = null;
			MaxSymbols ??= MaxValue.ToString().Length; //TODO: Добавить проверку для double
			if (MaxSymbols == 0) MaxSymbols = null;

			SUKcount = SingleUseKeys.Length;

			SUKused = new bool[SUKcount];
			SUKused.Initialize();

			string WhitelistKeys = "12345678790";
			foreach (char Key in SingleUseKeys)
			{
				WhitelistKeys += Key;
			}

			while (true)
			{
				Continue2:
				CurrentKey = ReadKey(true);
				//if (CurrentKey == '\0') continue;
				Stop = false;
				Read = false;

				if (CurrentKey.Key == ConsoleKey.Backspace)
				{
					if (InputString == "") continue;
					if (Print) Console.Write("\b \b");
					for (int i = 0; i < SUKcount; i++)
						if (InputString[InputString.Length - 1] == SUKcount)
							SUKused[i] = false;

					InputString = InputString.Remove(InputString.Length - 1);
					continue;
				} 

				foreach (var Key in StopReadKeys)
				{
					//список клавиш завершения ввода
					if (Key == CurrentKey.Key)
					{
						ro_keyPressed = Key;
						ro_keyType = ReadKeyType.StopKey;
						Stop = true;
						//if (ThrewStopKeys) InputString = InputString + ((int)CurrentKey.Key).ToString().FormatString(3,TypesFuncs.Positon.right,'0');
					}
				}

				if(!Stop) foreach (var Key in CancelReadKeys)
				{
					//список клавиш завершения ввода
					if (Key == CurrentKey.Key)
					{
						ro_keyPressed = Key;
						ro_keyType = ReadKeyType.CancelKey;
						Stop = true;
						//if (ThrewStopKeys) InputString = InputString + ((int)CurrentKey.Key).ToString().FormatString(3,TypesFuncs.Positon.right,'0');
					}
				}

				if(!Stop)
				{
					if (InputType != Input.String)
					{
						foreach (char Key in WhitelistKeys)
						{
							//список разрешенных клавиш
							if (Key == CurrentKey.KeyChar) Read = true;
						}

						for (int i = 0; i < SUKcount; i++)
							if (CurrentKey.KeyChar == SingleUseKeys[i])
								if (SUKused[i]) Read = false;
								else SUKused[i] = true;
					}
					else Read = true;

					if (Read)
					{
						InputString += CurrentKey.KeyChar;

						if (MaxSymbols!=null && (InputString.Length > MaxSymbols || 
						                         (InputString.Length == MaxSymbols && String.Compare(InputString, ((InputString[0]=='-')?MinValue : MaxValue).ToString()) == 1))) //: Вводимая строка больше MaxValue
						{
							Error($"Ввод игнорируется, поскольку вводимое число не может быть больше {MaxValue}",p,out isErrorPrinted);
							InputString = InputString.Remove(InputString.Length - 1);
						} 
						else if(Print) Console.Write(CurrentKey.KeyChar);
					}
					else
					{
						Error($"Клавиша \"{CurrentKey.KeyChar}\" недопустима",p,out isErrorPrinted);
					}



				} else break;
			}

			/*double? ro_double = null;
			long? ro_int = null;
			if (InputType is Input.Int or Input.UInt) ro_int = InputString.ToNIntT();
			if (InputType is Input.Double or Input.UDouble) ro_double = InputString.ToNDoubleT();
			if (InputType is Input.Long) ro_int= InputString.ToLongT();*/

			ReadOutput ro = new ReadOutput(ro_keyPressed, ro_keyType, InputString);

			Clr(DefaultTextColor);
			if (isErrorPrinted)
			{
				CPV cpv = p.ErrorPosition == int.MaxValue ? CPV.Bottom : p.ErrorPosition == int.MinValue ? CPV.Up : CPV.None;
				int LinesUp = cpv == CPV.None? p.ErrorPosition : 0;
				if (p.isErrorPosition_Absolute) cpv = CPV.Up;
				ReWrite(LinesUp:LinesUp,CurPosV:cpv, TextColor: p.ErrorTextColor,CurPosH:CPH.Left);
			}
			return ro;
		}

		public static string AskString(this ReadOutput ro) => ro.KeyType is ReadKeyType.CancelKey ? null : ro.String();
		public static double? AskDouble(this ReadOutput ro) => ro.KeyType is ReadKeyType.CancelKey ? null : ro.Double();
		public static int? AskInt(this ReadOutput ro) => ro.KeyType is ReadKeyType.CancelKey ? null : (int)ro.Int();
		public static long? AskLong(this ReadOutput ro) => ro.KeyType is ReadKeyType.CancelKey ? null : ro.Int();

		private class ReadT_PrintErrorParametrs
		{
			public bool ShowError = false;
			public string ErrorTextColor = "Red";
			public int ErrorPosition = Int32.MaxValue;
			public bool isErrorPosition_Absolute = false;

			public ReadT_PrintErrorParametrs(bool showError, string errorTextColor, int errorPosition, bool isErrorPositionAbsolute)
			{
				ShowError = showError;
				ErrorTextColor = errorTextColor;
				ErrorPosition = errorPosition;
				isErrorPosition_Absolute = isErrorPositionAbsolute;
			}
		}

		public static string ChangeRead(ref string InputString,Input InputType = Input.String, string StopReadKeys = "\n\r", bool ThrewStopKeys = false, bool Print = true, string TextColor = "Orange")
		{
			bool curvis = CursorVisible;
			CursorVisible = true;
			string OldColor = Console.ForegroundColor.Str();
			Clr(TextColor);
			if (InputType == Input.Bool) return qu(InputString).ToRuString();

			char CurrentKey;
			bool Stop, Read;
			string SingleUseKeys = "";
			bool[] SUKused;
			int SUKcount = 0;
			InputString ??= "";
			if (InputType == Input.Double || InputType == Input.UDouble)
			{
				SingleUseKeys += ',';
				SUKcount++;
			}

			if (InputType == Input.Int || InputType == Input.Double)
			{
				SingleUseKeys += '-';
				SUKcount++;
			}

			SUKused = new bool[SUKcount];
			SUKused.Initialize();

			string WhitelistKeys = "12345678790";
			foreach (char Key in SingleUseKeys)
			{
				WhitelistKeys += Key;
			}

			while (true)
			{
				Continue2:
				CurrentKey = ReadKey(true).KeyChar;
				if (CurrentKey == '\0') continue;
				Stop = false;
				Read = false;

				if (CurrentKey == '\b')
				{
					if (InputString == "") continue;
					if (Print) Console.Write("\b \b");
					for (int i = 0; i < SUKcount; i++)
						if (InputString[InputString.Length - 1] == SUKcount)
							SUKused[i] = false;

					InputString = InputString.Remove(InputString.Length - 1);
					continue;
				} 

				foreach (char Key in StopReadKeys)
				{
					//список клавиш завершения ввода
					if (Key == CurrentKey)
					{
						Stop = true;
						if (ThrewStopKeys) InputString = CurrentKey + InputString;
					}
				}

				if (InputType != Input.String)
				{
					foreach (char Key in WhitelistKeys)
					{
						//список разрешенных клавиш
						if (Key == CurrentKey) Read = true;
					}

					for (int i = 0; i < SUKcount; i++)
						if (CurrentKey == SingleUseKeys[i])
							if (SUKused[i]) Read = false;
							else SUKused[i] = true;
				}

				if (Stop && InputString != "") break;
				else if (Read || InputType ==  Input.String)
				{
					InputString += CurrentKey;
					if(Print) Console.Write(CurrentKey);
				}
			}

			Clr();
			CursorVisible = curvis;
			return InputString;
		}
		public enum Input //:08.11.2021
		{
			UInt,
			Int,
			Long,
			BigInteger,
			UDouble,
			Double,
			String,
			Bool,
			StringArray
		}

		public static void WaitKey(string TextColor = "null",  string BackgroundColor = "null", int LinesUp = -1, int ShiftRight = 0, bool ClearLine = true, CPH CurPosH = CPH.Left, CPV CurPosV = CPV.None, string ClearLineColor = "default", bool RestoreCurPos = false)
		{
			Consol.ReWrite("Нажмите любую клавишу, чтобы продолжить", LinesUp, ShiftRight,  ClearLine,  CurPosH,  CurPosV, TextColor,  BackgroundColor,  ClearLineColor,  RestoreCurPos);
			ReadKey();
			;		}
		#endregion

		#region Output

		/// <summary>
		/// Cursor position horisontal
		/// </summary>
		public enum CPH
		{
			None,
			Left,
			Right,
			Center,
			Absolute = Left
		}

		/// <summary>
		/// cursor position vertical
		/// </summary>
		public enum CPV
		{
			None,
			Up,
			Top = Up,
			Down,
			Bottom = Down,
			Center,
			Absolute = Up
		}



		public static void ClearWindow(int? LinesCount = null, int? StartPos = null)
		{
			RClr();
			ReWrite();
			if(StartPos != null) CursorTop = (int)StartPos;
			LinesCount ??= WindowHeight - CursorTop;
			for (int i = 0; i < LinesCount; i++)
			{
				CursorTop++;
				ReWrite(CurPosH:CPH.Left);
			}
		}

		/// <summary>
		/// Заменяет содержимое консоли строкой String, находящейся на LinesUp строк выше, чем текущая и на ShiftRight символов правее, с выравниванием по ширине curPosH и по высоте curPosV
		/// </summary>
		/// <param name="String">строка, которой заменяется текст у указанной позиции. Если не задано, то очищается строка срава от курсора</param>
		/// <param name="LinesUp">Насколько строк выше нужно поднять текст (отрицательные значения – опусить ниже)</param>
		/// <param name="ShiftRight">Изменение позиции курсора от текущего положения</param>
		/// <param name="ClearLine">Очистить ли строку, на которую будет выведен текст</param>
		/// <param name="CurPosH">Позиция текста по горизонтали (по умолчанию – не изменять)</param>
		/// <param name="CurPosV">Позиция текста по вертикали (по умолчанию – не изменять)</param>
		/// <param name="TextColor">Цвет текста String</param>
		/// <param name="BackgroundColor">Цвет фона String</param>
		/// <param name="ClearLineColor">Цвет фона пустой строки</param>
		public static void ReWrite(string String = "//JustCle@rLine", int LinesUp = 0, int ShiftRight = 0, bool ClearLine = false, CPH CurPosH = CPH.None, CPV CurPosV = CPV.None, string TextColor = "null", string BackgroundColor = "null", string ClearLineColor = "default", bool RestoreCurPos = false) 
		{ //:07.11.2021 Behavior changed: теперь RestoreCurPos игнорирует CPH

//bool curvi = CursorVisible;
//CursorVisible = true;
			
//TODO: реализовать остальные CursorPosition
			if(String == null) return;
			if (String.Contains('\n'))
			{
				string[] strings = String.Split('\n');

				for (int i = 0; i < strings.Length-1; i++)
				{
					ReWrite(strings[i],LinesUp,ShiftRight,ClearLine,CurPosH,CurPosV,TextColor,BackgroundColor,ClearLineColor);
					CursorTop++;
					CursorLeft = 0;
				}

				if (strings[^1].Length > 0) ReWrite(strings[^1],LinesUp,ShiftRight,ClearLine,CurPosH,CurPosV,TextColor,BackgroundColor,ClearLineColor);

				return;
			}

			if (String == "//JustCle@rLine")
			{
				String = "";
				ClearLine = true;
			}

			string fClr = Console.ForegroundColor.Str();
			string bClr = Console.BackgroundColor.Str();

			int curLeft = CursorLeft,
				curTop = CursorTop;
			switch (CurPosH) //TODO: добавить сброс на ноль или BufferWidth/WindowHeight, если выходит за этот диапазон
			{
				case CPH.None:
					CursorLeft += ShiftRight;
					break;

				case CPH.Left:
					CursorLeft = Math.Abs(ShiftRight);
					break;

				case CPH.Right:
					CursorLeft = BufferWidth - String.Length; //TODO: создать функцию для подсчёта печатных символов строки
					CursorLeft -= Math.Abs(ShiftRight);
					break;

				case CPH.Center:
					CursorLeft = (BufferWidth - String.SymbolsCount())/2;
					CursorLeft += ShiftRight;
					break;
			}

			switch (CurPosV)
			{
				case CPV.None:		break;
				case CPV.Up:		CursorTop = 0; break;
				case CPV.Bottom:	CursorTop = WindowHeight - 1; break;
				case CPV.Center:	CursorTop = (WindowHeight - 1) / 2; break;
			}

			if (CurPosV == CPV.Absolute) CursorTop += Math.Abs(LinesUp);
			else CursorTop -= LinesUp;

			Clr(TextColor,BackgroundColor);
			Console.Write(String);


			if (ClearLine)
			{
				Consol.BClr(ClearLineColor);
				
				Console.Write(new string(' ',BufferWidth - CursorLeft));
			}

			Clr(fClr, bClr);

			try
			{
				if (CurPosV == CPV.None)
					CursorTop += LinesUp;
				else CursorTop = curTop;
			}
			catch (Exception e)
			{
				CursorTop = 0;
			}

			if (RestoreCurPos) CursorLeft = curLeft;
//CursorVisible = curvi;
		}

		/// <summary>
		/// Заменяет содержимое консоли строками Strings, каждая из которых имеет соответствующий TextColors и BackgroundColors, находящейся на LinesUp строк выше, чем текущая и на ShiftRight символов правее
		/// </summary>
		/// <param name="String">строка, которой заменяется текст у указанной позиции</param>
		/// <param name="LinesUp">Насколько строк выше нужно поднять текст (отрицательные значения – опусить ниже)</param>
		/// <param name="ShiftRight">Изменение позиции курсора от текущего положения</param>
		/// <param name="ClearLine">Очистить ли строку, на которую будет выведен текст</param>
		/// <param name="CurPosH">Позиция текста по горизонтали (по умолчанию – не изменять)</param>
		/// <param name="CurPosV>Позиция текста по вертикали (по умолчанию – не изменять)</param>
		/// <param name="TextColor">Цвет текста String</param>
		/// <param name="BackgroundColor">Цвет фона String</param>
		/// <param name="ClearLineColor">Цвет фона пустой строки</param>
		public static void ReWrite(string[] Strings, string[] TextColors = null, string[] BackgroundColors = null, int LinesUp = 0, int ShiftRight = 0, bool ClearLine = false, CPH CurPosH = CPH.None, CPV CurPosV = CPV.None, string ClearLineColor = "default", bool RestoreCurPos = false)
		{ //:29.11.2021 Изменено поведение CPV и CPH, \n
			//TODO: объединить оба ReWrite, создав два метода до и после Write
			int StringsLength = Strings.Sum(s => s.Length);
			TextColors??= SetDefaultColors(TextColors,Strings.Length);
			BackgroundColors??= SetDefaultColors(BackgroundColors,Strings.Length);

			string[] SetDefaultColors(string[] Colors, int Length)
			{
				Colors = new string[Length];
					for (int i = 0; i < Length; i++)
					{
						Colors[i] = "null";
					}

					return Colors;
			}

			if (!(Strings.Length == TextColors.Length && Strings.Length == BackgroundColors.Length))
				throw new ArgumentOutOfRangeException($"Strings[{Strings.Length}] should be same size as TextColors[{TextColors.Length}] and BackgroundColors[{BackgroundColors.Length}]");

			//bool curvi = CursorVisible;
//CursorVisible = true;
			/*if (String.Contains('\n'))
			{
				string[] strings = String.Split('\n');

				for (int i = 0; i < strings.Length-1; i++)
				{
					ReWrite(strings[i],LinesUp,ShiftRight,ClearLine,CurPosH,CurPosV,TextColors,BackgroundColor,ClearLineColor);
					CursorTop++;
				}

				if (strings[^1].Length > 0) ReWrite(strings[^1],LinesUp,ShiftRight,ClearLine,CurPosH,CurPosV,TextColor,BackgroundColor,ClearLineColor);

				return;
			}*/

			string fClr = Console.ForegroundColor.Str();
			string bClr = Console.BackgroundColor.Str();

			int curLeft = CursorLeft,
				curTop = CursorTop;
			switch (CurPosH) //TODO: добавить сброс на ноль или BufferWidth/WindowHeight, если выходит за этот диапазон
			{
				case CPH.None:
					CursorLeft += ShiftRight;
					break;

				case CPH.Left:
					CursorLeft = Math.Abs(ShiftRight);
					break;

				case CPH.Right:
					CursorLeft = BufferWidth - StringsLength; //TODO: создать функцию для подсчёта печатных символов строки
					CursorLeft -= Math.Abs(ShiftRight);
					break;

				case CPH.Center:
					CursorLeft = (BufferWidth - Strings.Sum(s => s.SymbolsCount()))/2;
					CursorLeft += ShiftRight;
					break;
			}

			switch (CurPosV)
			{
				case CPV.None:		CursorTop -= LinesUp; break;
				case CPV.Up:		CursorTop = LinesUp; break;
				case CPV.Bottom:	CursorTop = WindowHeight - 1 - LinesUp; break;
				case CPV.Center:	CursorTop = (WindowHeight - 1) / 2 - LinesUp; break;
			}

			for (int i = 0; i < Strings.Length; i++)
			{
				Clr(TextColors[i],BackgroundColors[i]);
				Console.Write(Strings[i]);
				if (ClearLine&&(Strings[i].Contains("\n")||(i==Strings.Length-1&&Strings[i]!="\n")))
				{
					Consol.BClr(ClearLineColor);
				
					Console.Write(new string(' ',BufferWidth - CursorLeft));
				}
			}
			


			Clr(fClr, bClr);

			try
			{
				if (CurPosV == CPV.None)
					CursorTop += LinesUp;
				else CursorTop = curTop;
			}
			catch (Exception e)
			{
				CursorTop = 0;
			}
			

			if (RestoreCurPos) CursorLeft = curLeft;
			else
				try
				{
					if (CurPosH == CPH.None)
						if (ClearLine) CursorLeft = curLeft +ShiftRight + StringsLength;
						/*else CursorLeft -= (ShiftRight + StringsLength);
					else CursorLeft = curLeft;*/
				}
				catch (Exception e)
				{
					CursorLeft = 0;
				}
//CursorVisible = curvi;
		}

		public static void ReWrite(string[] Strings, string[] TextColors, nString BackgroundColor, int LinesUp = 0, int ShiftRight = 0, bool ClearLine = false, CPH CurPosH = CPH.None, CPV CurPosV = CPV.None, string ClearLineColor = "default", bool RestoreCurPos = false)
		{
			string[] BackgroundColors = new string[TextColors.Length];

			for (int i = 0; i < BackgroundColors.Length; i++)
			{
				BackgroundColors[i] = BackgroundColor;
			}

			ReWrite(Strings,TextColors,BackgroundColors,LinesUp,ShiftRight,ClearLine,CurPosH,CurPosV,ClearLineColor,RestoreCurPos);
		}

		public class Loadbar
		{
			internal string BorderLeftSymbol{ get; }
			internal string BorderLeftColor { get; }
			internal string BorderRightSymbol{ get; }
			internal string BorderRightColor { get; }
			internal string LineSymbol { get; }
			internal string LineColor { get; }
			internal string EmptyLineSymbol  { get; }
			public int MaxLoadLineLength { get; }
			public int LoadbarPositionLeft { get; internal set; }
			internal int Cursor { get; set; }
			public int LoadbarPositionTop { get; internal set; }
			internal int CurrentLoadlineLength { get; set; }


			public Loadbar(int LoadlineLength = 10, string borderLeftSymbol = "[", string borderRightSymbol = "]", string lineSymbol = "—", string emptySymbol = " ", string borderLeftColor = "Cyan", string borderRightColor = "Cyan", string lineColor = "Lime")
			{
				if (lineSymbol.Length < 1) throw new ArgumentException("lineSymbol should have at least 1 symbol");
				if (lineSymbol.Length != emptySymbol.Length) throw new ArgumentException("lineSymbol and emptySymbol should be same length");
				LoadbarPositionLeft = CursorLeft;
				LoadbarPositionTop = CursorTop;
				Cursor = CursorLeft + BorderLeftSymbol.Length;
				CurrentLoadlineLength = 0;

				LineSymbol = lineSymbol;
				EmptyLineSymbol = emptySymbol;
				BorderLeftSymbol = borderLeftSymbol;
				BorderRightSymbol = borderRightSymbol;
				int MaxPossibleLineLength = (Console.WindowWidth - (LoadbarPositionLeft + borderLeftSymbol.Length + borderRightSymbol.Length)) / lineSymbol.Length;
				MaxLoadLineLength = Math.Min(LoadlineLength*LineSymbol.Length, MaxPossibleLineLength);
				BorderLeftColor = borderLeftColor == "Cyan" ? Colors.Cyan.Str() : borderLeftColor;
				BorderRightColor = borderRightColor == "Cyan" ? Colors.Cyan.Str() : borderRightColor;
				LineColor = lineColor == "Lime" ? Colors.Lime.Str() : lineColor;
			}
		}

		public static void Reset(this Loadbar l, bool ResetPosition = false)
		{
			l.CurrentLoadlineLength = 0;
			if (ResetPosition)
			{
				l.LoadbarPositionLeft = CursorLeft;
				l.LoadbarPositionTop = CursorTop;
			}
			l.Write();
		}

		public static void Write(this Loadbar l, int Increments = 0, bool ClearLine = false)
		{
			if (Increments < 0) Increments = 0;
			{
				int MaxIncrements = l.MaxLoadLineLength - l.CurrentLoadlineLength;
			}
			if (Increments > l.MaxLoadLineLength) Increments = l.MaxLoadLineLength;
			if (l.CurrentLoadlineLength == 0)
			{
				if (l == null) throw new ArgumentNullException("LoadbarSettings can't be null");
				if (l.CurrentLoadlineLength == 0) ReWrite(new[] { l.BorderLeftSymbol, l.EmptyLineSymbol.Multiply(l.MaxLoadLineLength), l.BorderRightSymbol }, new[] { l.BorderLeftColor, l.LineColor, l.BorderRightColor }, Colors.DefBgr.Str(), -l.LoadbarPositionTop, l.LoadbarPositionLeft, ClearLine, CPH.Left, CPV.Up, RestoreCurPos: true);
			}

			if (Increments > 0)
			{
				int cl = CursorLeft, ct = CursorTop;
				ReWrite(l.LineSymbol.Multiply(Increments),l.LoadbarPositionTop,l.Cursor,ClearLine,CPH.Left,CPV.Up,l.LineColor);
				l.CurrentLoadlineLength = CursorLeft - l.LoadbarPositionLeft;
				l.Cursor = CursorLeft;
				CursorLeft = cl;
			}
		}

		public static void Write(this Loadbar l, float AddPercent)
		{
			if (AddPercent < 0) AddPercent = 0;
			if (AddPercent > 100) AddPercent = 100;
			Write(l, (int)(l.MaxLoadLineLength*(AddPercent/100)));
		}

		public static void Error(string ErrorText, string ActionText = "продолжить")
		{
			Console.Write("\nОшибка: " + ErrorText, Colors.Red); Console.Write(". Нажмите любую клавишу, чтобы "+ActionText, Colors.DefText);
			ReadKey();
		}

		public static bool Error(bool Term, string ErrorText, string ActionText = "продолжить")
		{
			if (Term)
			{
				Error(ErrorText, ActionText);
				return true;
			}

			return false;
		}

		public static void Error(string ErrorText,int SleepTime, string ActionText = "продолжить")
		{
			Console.Write("Ошибка: " + ErrorText, Colors.Red); Console.Write(". Нажмите любую клавишу, чтобы "+ActionText, Colors.DefText);
			Thread.Sleep(SleepTime);
		}

		public static bool Error(bool Term, string ErrorText, int SleepTime, string ActionText = "продолжить") 
		{
			if (Term)
			{
				Error(ErrorText,SleepTime,ActionText);
				return true;
			}

			return false;
		}

		#endregion

		#region Complex

		/// <summary>
		/// Writes menu from current cursor position
		/// </summary>
		/// <param name="Options">Menu options. First and last should always be active</param>
		/// <param name="cursor">Current selected option</param>
		/// <param name="TipString">Подсказка, которая отображается внизу, на последней строке консоли</param>
		/// <param name="TipStringColor"></param>
		/// <param name="CancelKeys">Кнопки, которые отменяют выбор опциию. По умолчанию, их нет</param>
		/// <param name="SelectKeys">Кнопки, которые выберают опцию. По умолчанию, Enter и Spacebar (пробел)</param>
		/// <param name="ClearLine">Очистить ли все строки, где будет напечатано меню? Если да, то выделение текущей опции распространиться на всю строку</param>
		/// <returns>Chosed option or -CancelKey if pressed</returns>
		public static int Menu(Option[] Options, int cursor = 0,string TipString = null, string TipStringColor = "Green", ConsoleKey[] CancelKeys = null, ConsoleKey[] SelectKeys = null, bool ClearLine = false, bool SelectWholeLine = false) //:28.11.21
		{
			//BUG: Исправить ClearLine баг, доделать поддержку ClearLine
			SelectKeys ??= new[] {ConsoleKey.Enter, ConsoleKey.Spacebar};
			if (TipStringColor == "Green") TipStringColor = Colors.Green.Str();

			CursorVisible = false;
			int curLeftStart = CursorLeft,
				curTopStart = CursorTop;

			ConsoleKeyInfo curKey;
			//: Вывод опций на экран
			for (int i = 0; i < Options.Length; i++)
			{
				bool active = Options[i]._active;
				ReWrite(new []{i + ". ",Options[i]._text + "\n"}, new []{(active ? Colors.Orange : Colors.Gray).Str(),(active ? Colors.DefText : Colors.Gray).Str()}, (i==cursor ? Colors.Select : Colors.DefBgr).Str(),ClearLine:ClearLine,ClearLineColor:SelectWholeLine? Colors.Select.Str() : Colors.DefText.Str());
				
			}

			int LastConsoleLine =  Console.WindowHeight - 1;
			ReWrite(TipString, LastConsoleLine,0,ClearLine,CPH.None,CPV.Top,TipStringColor);

			bool Exit = false;
			//: Выбор опции (навигация в меню)
			do
			{
				CursorVisible = false; //: Почему-то иногда сбрасывается, поэтому в цикле
				int oldCur = cursor;
				curKey = ReadKey(true);
				switch (curKey.Key)
				{
					case ConsoleKey.UpArrow:
					{
						if (cursor > 0)
						{
							do
							{
								cursor--;
							} while (!Options[cursor]._active);
						}
					}
						break;

					case ConsoleKey.DownArrow:
					{
						if (cursor < Options.Length - 1)
						{
							do
							{
								cursor++;
							} while (!Options[cursor]._active);
						}
					}
						break;

					default:
					{
						if (curKey.KeyChar.IsDigit())
						{
							cursor = curKey.KeyChar.ToIntT();
							if (!Options[cursor]._active) cursor = oldCur; //:Если опция неактивна, отменить выбор
							else Exit = true;
						}

						if (CancelKeys != null && CancelKeys.Contains(curKey.Key))
						{
							return -curKey.KeyChar;
						}
					}
						break;
				}

				if (cursor != oldCur)
				{
					ReWrite(new []{oldCur + ". ",Options[oldCur]._text}, new []{Colors.Orange.Str(), Colors.DefText.Str()},Colors.DefBgr.Str(), curTopStart + oldCur,curLeftStart,ClearLine,CPH.Absolute, CPV.Absolute,""); //:Затирание выделения предыдущей строки
					ReWrite(new []{cursor + ". ",Options[cursor]._text}, new []{Colors.Orange.Str(), Colors.DefText.Str()},Colors.Select.Str(), curTopStart + cursor,curLeftStart,ClearLine,CPH.Absolute, CPV.Absolute,""); //:Выделение новой строки
				}

			} while ((!SelectKeys.Contains(curKey.Key))&&!Exit);

			CursorVisible = true;
			return cursor;
		}

		/// <summary>
		/// Меню изменения настроек
		/// </summary>
		/// <param name="settings"></param>
		public static void Menu(ref Setting[] settings, bool ClearLine = true)
		{

			if (settings.Length < 1)
			{
				ReWrite("\nНастроек не найдено", TextColor: Colors.Red.Str());
				return;
			}

			CursorVisible = false;
			int curLeftStart = 0,
				curTopStart = CursorTop;

			CursorLeft = 0;
			int cursor = 0;
			ConsoleKeyInfo curKey;
			//Вывод опций на экран
			for (int i = 0; i < settings.Length; i++)
			{
				if (i == cursor)
					if (settings[i].Disabled)
						if (cursor<settings.Length)
						{
							cursor++; continue;
						}
						else
						{
							ReWrite("Все настройки недоступны для изменения");
							return;
						}


				var ColorText = (settings[i].Disabled ? Colors.Gray : Colors.White).Str();
				var ColorValue = (settings[i].Disabled? Colors.Gray : Colors.Yellow).Str();

				ReWrite(new []{settings[i].Text+": ",settings[i].Value,settings[i].Unit+"\n"}, new []{ColorText, ColorValue, ColorText});
				

				if (i == cursor)
				{
					BClr();
					ReWrite(String: settings[i].Description, CurPosH: CPH.Left, CurPosV: CPV.Bottom, TextColor: Colors.Green.Str());
				}
			}



			//Выбор опции (навигация в меню)
			string GetCurrentListValueString(int currentListIndex1, string[] strings)
			{
				string s;
				if (currentListIndex1 != 0) s = "← ";
				s = strings[currentListIndex1 + 1];
				if (currentListIndex1 != strings.Length - 1) s += " →";
				return s;
			}

			do
			{
				string[] list = null;
				int oldCur = cursor;
				curKey = ReadKey(true);
				switch (curKey.Key)
				{
					case ConsoleKey.UpArrow:
					{
						if (cursor > 0)
						{
							do
							{
								cursor--;
							} while (settings[cursor].Disabled && cursor <= settings.Length-1); //TODO: добавить проверку на конец массива
						}
					} break;

					case ConsoleKey.DownArrow:
					{
						if (cursor < settings.Length - 1)
						{
							do
							{
								cursor++;
							} while (settings[cursor].Disabled && cursor < settings.Length-1);
						}
					} break;

					case ConsoleKey.Enter:
						var valueCurLeft = CursorLeft;
						ref var curSetting = ref settings[cursor];
						ref var value = ref curSetting.Value;
						BClr(Colors.WeakSelect.Str());
						ReWrite( curSetting.Text+": ", CurPosH: CPH.Left, TextColor: Colors.White.Str());
						string valStr;
						
						if (curSetting.Type == Input.StringArray)
						{
							list = value.Split('\0');
							int currentListIndex = list[0].ToIntT();
							valStr = GetCurrentListValueString(currentListIndex, list);
						}
						else if (curSetting.Type == Input.Bool) valStr = value.ToBool() ? "← Да " : "Нет →";
						else valStr = value;
						
						ReWrite(valStr,0,-(value.Length+curSetting.Unit.Length), true,TextColor: Colors.Orange.Str());
						CursorLeft -= curSetting.Unit.Length;

						ReWrite("Нажмите Enter, чтобы завершить ввод",0,0,  true,  CPH.Left,  CPV.Bottom,Colors.Gray.Str(),"default");
						if (curSetting.Type == Input.StringArray)
						{
							int currentListIndex = list[0].ToIntT();
							ConsoleKey key;
							do
							{
								key = ReadKey(true).Key;
								switch (key)
								{
									case ConsoleKey.LeftArrow:
										if(currentListIndex != 0) currentListIndex--;
									break;

									case ConsoleKey.RightArrow:
										if (currentListIndex != list.Length - 2) currentListIndex++;

									break;
								}

								CursorLeft = valueCurLeft;
								ReWrite(GetCurrentListValueString(currentListIndex, list), TextColor: Colors.Yellow.Str());
							} while (key!=ConsoleKey.Enter&&key!=ConsoleKey.Escape);

							ReWrite(new []{settings[cursor].Text+": ", settings[cursor].Value, settings[cursor].Unit}, new []{Colors.DefText.Str(), Colors.Yellow.Str(), Colors.DefText.Str()}, Colors.Select.Str(),ClearLine:true, CurPosH:CPH.Left);
						} 
						else if (curSetting.Type == Input.Bool)
						{
							ConsoleKey key;
							do
							{
								Clr(Colors.Orange.Str());
								key = ReadKey(true).Key;
								switch (key)
								{
									case ConsoleKey.LeftArrow:
										if (value.ToBool() == true)
										{
											value = "Нет";
											ReWrite("Нет →", 0, -5);
										}

										break;

									case ConsoleKey.RightArrow:
										if (value.ToBool() == false)
										{
											value = "Да";
											ReWrite("← Да ", 0, -5);
										}

										break;
								}
							} while (key!=ConsoleKey.Enter&&key!=ConsoleKey.Escape);
						
							ReWrite(new []{settings[cursor].Text+": ", settings[cursor].Value, settings[cursor].Unit}, new []{Colors.DefText.Str(),Colors.Yellow.Str(),Colors.DefText.Str()}, CurPosH:CPH.Left,ClearLine:true);
						}
						else ChangeRead(ref value, curSetting.Type);

						break;

					default:
					{
					}
						break;
				}

				if (cursor != oldCur)
				{

					ReWrite(new []{settings[oldCur].Text+": ", settings[oldCur].Value,settings[oldCur].Unit}, new []{Colors.DefText.Str(),Colors.Orange.Str(), Colors.DefText.Str()},Colors.DefBgr.Str(), curTopStart + oldCur,curLeftStart,ClearLine,CPH.Absolute, CPV.Absolute,""); //:Затирание выделения предыдущей строки
					ReWrite(new []{settings[oldCur].Text+": ", settings[oldCur].Value,settings[oldCur].Unit}, new []{Colors.DefText.Str(),Colors.Orange.Str(), Colors.DefText.Str()},Colors.Select.Str(), curTopStart + cursor,curLeftStart,ClearLine,CPH.Absolute, CPV.Absolute,""); //:Выделение новой строки
					ReWrite(String: settings[cursor].Description,ClearLine: true, CurPosH: CPH.Left, CurPosV: CPV.Bottom, TextColor: Colors.Green.Str()); //:Вывод подсказки
				}

			} while (curKey.Key != ConsoleKey.Escape);

			CursorVisible = true;
		}

		#endregion
		
		
		

		
	}
}

	public static class SettingsFunctions
	{
		public static void Enable(this Setting[] S, string name)
		{
			foreach (var s in S)
			{
				s.Enable(name);
			}
		}
		public static void Disable(this Setting[] S, string name)
		{
			foreach (var s in S)
			{
				s.Disable(name);
			}
		}
		public static string Get(this Setting[] S, string name)
		{
			string R;
			foreach (var s in S)
			{
				R = s.Get(name);
				if (R != null) return R;
			}

			return null;
		}
	}

	public class Setting //TODO: Добавить dependency
	{
		public string Text;
		public string Description;
		public string Name;
		public Consol.Input Type;
		public string Value;
		public string Unit; //:the unit of the value || Единица измерения
		public bool Disabled;

		/// <summary>
		/// Стандартный конструктор для создания опции настроек
		/// </summary>
		/// <param name="name">Имя параметра</param>
		/// <param name="type">Тип</param>
		/// <param name="value">Значение по умолчанию</param>
		/// <param name="description">Описание</param>
		/// <param name="unit">Единица измерения</param>
		/// <param name="disabled">Доступен ли параметр для изменения</param>
		/// <param name="Text">Выводимый текст, при выборе параметра (если не выбрано, то = имени параметра)</param>
		public Setting(string name, Consol.Input type = Consol.Input.Int, string value = null, string description = "", string unit = "", bool disabled = false, string Text =  null)
		{
			Text ??= name;
			Name = name;
			Type = type;
			Description = description;
			Unit = unit;
			Value = value;
			Disabled = disabled;
		}
		
		/// <summary>
		/// Конструктор для типа StringArray (выпадающего списка)
		/// </summary>
		///  <param name="name">Имя параметра</param>
		/// <param name="listValues">Список возможных значений</param>
		/// <param name="defaultValue">Порядковый номер значения по умолчанию</param>
		///<param name="description">Описание</param>
		/// <param name="unit">Единица измерения</param>
		/// <param name="disabled">Доступен ли параметр для изменения</param>
		/// <param name="Text">Выводимый текст, при выборе параметра (если не выбрано, то = имени параметра)</param>
		public Setting(string name, string[] listValues, int defaultValue = 0, string description = "", string unit = "", bool disabled = false, string Text =  null)
		{
			if (defaultValue >= listValues.Length) throw new ArgumentOutOfRangeException($"defaultValue ({defaultValue}) can't be more than the count of listValues ({listValues.Length})");
			if (listValues.Length<=1) throw  new ArgumentOutOfRangeException("listValues length should be more than 1");

			Text ??= name;
			Name = name;
			Type = Consol.Input.StringArray;
			Description = description;
			Unit = unit;
			Value = defaultValue + "\0" + String.Join('\0', listValues);
			Disabled = disabled;
		}

		/// <summary>
		/// Конструктор для типа StringArray (выпадающего списка) с отдельным описанием для каждой опции
		/// </summary>
		/// <param name="name">Имя параметра</param>
		/// <param name="listValues">Список возможных значений</param>
		/// <param name="defaultValue">Порядковый номер значения по умолчанию</param>
		///<param name="descriptions">Описание для каждого параметра.</param>
		/// <param name="unit">Единица измерения</param>
		/// <param name="disabled">Доступен ли параметр для изменения</param>
		/// <param name="Text">Выводимый текст, при выборе параметра (если не выбрано, то = имени параметра)</param>
		/// <param name="sharedDescription">Общее описание. Вставляется вначале каждого отдельного описания (если есть)</param>
		public Setting(string name, string[] listValues, int defaultValue = 0, string[] descriptions = null, string unit = "", bool disabled = false, string sharedDescription = null, char Separator = ' ', string Text =  null)
		{
			if (defaultValue >= listValues.Length) throw new ArgumentOutOfRangeException($"defaultValue ({defaultValue}) can't be more than the count of listValues ({listValues.Length})");
			if (listValues.Length<=1) throw  new ArgumentOutOfRangeException("listValues length should be more than 1");
			if (descriptions == null||descriptions.Length <=1) throw new ArgumentOutOfRangeException("description length should be more than 1");

			Text ??= name;
			Name = name;
			Type = Consol.Input.StringArray;
			Description = "\0" + String.Join('\0', descriptions) + ((sharedDescription == null)? ' ' + sharedDescription + "\0" : "");
			Unit = unit;
			Value = defaultValue + "\0" + String.Join('\0', listValues) + "\0";
			Disabled = disabled;
		}

		public string Get(string name)
		{
			if (Name == name) return Value;
			
			return null;
		}

		public string Get()
		{
			if (Type != Consol.Input.StringArray) return Value;
			var list = Value.Split('\0');

			return list[list[0].ToIntT() + 1];
		}

		public void Enable(string name)
		{
			if (Name == name) Disabled = false;
		}
		public void Disable(string name)
		{
			if (Name == name) Disabled = true;
		}
	}

	public static class OptionsFunctions
	{
		public static void Activate(this Option[] ops, int dependency)
		{
			foreach (var op in ops)
			{
				op.Activate(dependency);
			}
		}
		public static void Deactivate(this Option[] ops, int dependency)
		{
			foreach (var op in ops)
			{
				op.Deactivate(dependency);
			}
		}
	}

	public class Option
	{
		public string _text;
		public int _dependency;
		public bool _active;
		public string _name;

		public Option(string text, string name = null, int dependency = 0, bool? active = null)
		{
			_text = text;
			_name = name;
			_dependency = dependency;
			_active = active?? dependency == 0; //:if active unset, set true if dependecy == 0
		}

		public void Activate() => _active = true;
		public void Activate(int dependency)
		{
			if (dependency == _dependency) _active = true;
		}
		public void Activate(string Name)
		{
			if (Name == _name) _active = true;
		}
		
		public void Deactivate() => _active = false;
		public void Deactivate(int dependency)
		{
			if (dependency == _dependency) _active = false;
		}
		public void Deactivate(string Name)
		{
			if (Name == _name) _active = false;
		}
	}


//!Чужой код

/// <summary>
/// Non-nullable string.
/// <author>https://stackoverflow.com/users/2061451/jared-price</author>
/// </summary>
public struct nString
{
    public nString(string value)
        : this()
    {
        Value = value ?? "";
    }

    public nString(char[] value)
    {
        Value = new string(value) ?? "";
    }

    public nString(char c, int count)
    {
        Value = new string(c, count) ?? "";
    }

    public nString(char[] value, int startIndex, int length)
    {
        Value = new string(value, startIndex, length) ?? "";
    }

    public string Value
    {
        get;
        private set;
    }

    public static implicit operator nString(string value)
    {
        return new nString(value);
    }

    public static implicit operator string(nString value)
    {
        return value.Value ?? "";
    }

    public int CompareTo(string strB)
    {
        Value = Value ?? "";
        return Value.CompareTo(strB);
    }

    public bool Contains(string value)
    {
        Value = Value ?? "";
        return Value.Contains(value);
    }

    public void CopyTo(int sourceIndex, char[] destination, int destinationIndex, int count)
    {
        Value = Value ?? "";
        Value.CopyTo(sourceIndex, destination, destinationIndex, count);
    }

    public bool EndsWith(string value)
    {
        Value = Value ?? "";
        return Value.EndsWith(value);
    }

    public bool EndsWith(string value, StringComparison comparisonType)
    {
        Value = Value ?? "";
        return Value.EndsWith(value, comparisonType);
    }

    public override bool Equals(object obj)
    {
        Value = Value ?? "";
        return Value.Equals(obj);
    }

    public bool Equals(string value)
    {
        Value = Value ?? "";
        return Value.Equals(value);
    }

    public bool Equals(string value, StringComparison comparisonType)
    {
        Value = Value ?? "";
        return Value.Equals(value, comparisonType);
    }

    public override int GetHashCode()
    {
        Value = Value ?? "";
        return Value.GetHashCode();
    }

    public new Type GetType()
    {
        return typeof(string);
    }

    public int IndexOf(char value)
    {
        Value = Value ?? "";
        return Value.IndexOf(value);
    }

    public int IndexOf(string value)
    {
        Value = Value ?? "";
        return Value.IndexOf(value);
    }

    public int IndexOf(char value, int startIndex)
    {
        Value = Value ?? "";
        return Value.IndexOf(value, startIndex);
    }

    public int IndexOf(string value, int startIndex)
    {
        Value = Value ?? "";
        return Value.IndexOf(value, startIndex);
    }

    public int IndexOf(string value, StringComparison comparisonType)
    {
        Value = Value ?? "";
        return Value.IndexOf(value, comparisonType);
    }

    public int IndexOf(char value, int startIndex, int count)
    {
        Value = Value ?? "";
        return Value.IndexOf(value, startIndex, count);
    }

    public int IndexOf(string value, int startIndex, int count)
    {
        Value = Value ?? "";
        return Value.IndexOf(value, startIndex, count);
    }

    public int IndexOf(string value, int startIndex, StringComparison comparisonType)
    {
        Value = Value ?? "";
        return Value.IndexOf(value, startIndex, comparisonType);
    }

    public int IndexOf(string value, int startIndex, int count, StringComparison comparisonType)
    {
        Value = Value ?? "";
        return Value.IndexOf(value, startIndex, count, comparisonType);
    }

    public int IndexOfAny(char[] anyOf)
    {
        Value = Value ?? "";
        return Value.IndexOfAny(anyOf);
    }

    public int IndexOfAny(char[] anyOf, int startIndex)
    {
        Value = Value ?? "";
        return Value.IndexOfAny(anyOf, startIndex);
    }

    public int IndexOfAny(char[] anyOf, int startIndex, int count)
    {
        Value = Value ?? "";
        return Value.IndexOfAny(anyOf, startIndex, count);
    }

    public string Insert(int startIndex, string value)
    {
        Value = Value ?? "";
        return Value.Insert(startIndex, value);
    }

    public int LastIndexOf(char value)
    {
        Value = Value ?? "";
        return Value.LastIndexOf(value);
    }

    public int LastIndexOf(string value)
    {
        Value = Value ?? "";
        return Value.LastIndexOf(value);
    }

    public int LastIndexOf(char value, int startIndex)
    {
        Value = Value ?? "";
        return Value.LastIndexOf(value, startIndex);
    }

    public int LastIndexOf(string value, int startIndex)
    {
        Value = Value ?? "";
        return Value.LastIndexOf(value, startIndex);
    }

    public int LastIndexOf(string value, StringComparison comparisonType)
    {
        Value = Value ?? "";
        return Value.LastIndexOf(value, comparisonType);
    }

    public int LastIndexOf(char value, int startIndex, int count)
    {
        Value = Value ?? "";
        return Value.LastIndexOf(value, startIndex, count);
    }

    public int LastIndexOf(string value, int startIndex, int count)
    {
        Value = Value ?? "";
        return Value.LastIndexOf(value, startIndex, count);
    }

    public int LastIndexOf(string value, int startIndex, StringComparison comparisonType)
    {
        Value = Value ?? "";
        return Value.LastIndexOf(value, startIndex, comparisonType);
    }

    public int LastIndexOf(string value, int startIndex, int count, StringComparison comparisonType)
    {
        Value = Value ?? "";
        return Value.LastIndexOf(value, startIndex, count, comparisonType);
    }

    public int LastIndexOfAny(char[] anyOf)
    {
        Value = Value ?? "";
        return Value.LastIndexOfAny(anyOf);
    }

    public int LastIndexOfAny(char[] anyOf, int startIndex)
    {
        Value = Value ?? "";
        return Value.LastIndexOfAny(anyOf, startIndex);
    }

    public int LastIndexOfAny(char[] anyOf, int startIndex, int count)
    {
        Value = Value ?? "";
        return Value.LastIndexOfAny(anyOf, startIndex, count);
    }

    public int Length
    {
        get
        {
            Value = Value ?? "";
            return Value.Length;
        }
    }

    public string PadLeft(int totalWidth)
    {
        Value = Value ?? "";
        return Value.PadLeft(totalWidth);
    }

    public string PadLeft(int totalWidth, char paddingChar)
    {
        Value = Value ?? "";
        return Value.PadLeft(totalWidth, paddingChar);
    }

    public string PadRight(int totalWidth)
    {
        Value = Value ?? "";
        return Value.PadRight(totalWidth);
    }

    public string PadRight(int totalWidth, char paddingChar)
    {
        Value = Value ?? "";
        return Value.PadRight(totalWidth, paddingChar);
    }

    public string Remove(int startIndex)
    {
        Value = Value ?? "";
        return Value.Remove(startIndex);
    }

    public string Remove(int startIndex, int count)
    {
        Value = Value ?? "";
        return Value.Remove(startIndex, count);
    }

    public string Replace(char oldChar, char newChar)
    {
        Value = Value ?? "";
        return Value.Replace(oldChar, newChar);
    }

    public string Replace(string oldValue, string newValue)
    {
        Value = Value ?? "";
        return Value.Replace(oldValue, newValue);
    }

    public string[] Split(params char[] separator)
    {
        Value = Value ?? "";
        return Value.Split(separator);
    }

    public string[] Split(char[] separator, StringSplitOptions options)
    {
        Value = Value ?? "";
        return Value.Split(separator, options);
    }

    public string[] Split(string[] separator, StringSplitOptions options)
    {
        Value = Value ?? "";
        return Value.Split(separator, options);
    }

    public bool StartsWith(string value)
    {
        Value = Value ?? "";
        return Value.StartsWith(value);
    }

    public bool StartsWith(string value, StringComparison comparisonType)
    {
        Value = Value ?? "";
        return Value.StartsWith(value, comparisonType);
    }

    public string Substring(int startIndex)
    {
        Value = Value ?? "";
        return Value.Substring(startIndex);
    }

    public string Substring(int startIndex, int length)
    {
        Value = Value ?? "";
        return Value.Substring(startIndex, length);
    }

    public char[] ToCharArray()
    {
        Value = Value ?? "";
        return Value.ToCharArray();
    }

    public string ToLower()
    {
        Value = Value ?? "";
        return Value.ToLower();
    }

    public string ToLowerInvariant()
    {
        Value = Value ?? "";
        return Value.ToLowerInvariant();
    }

    public override string ToString()
    {
        Value = Value ?? "";
        return Value.ToString();
    }

    public string ToUpper()
    {
        Value = Value ?? "";
        return Value.ToUpper();
    }

    public string ToUpperInvariant()
    {
        Value = Value ?? "";
        return Value.ToUpperInvariant();
    }

    public string Trim()
    {
        Value = Value ?? "";
        return Value.Trim();
    }

    public string Trim(params char[] trimChars)
    {
        Value = Value ?? "";
        return Value.Trim(trimChars);
    }

    public string TrimEnd(params char[] trimChars)
    {
        Value = Value ?? "";
        return Value.TrimEnd(trimChars);
    }

    public string TrimStart(params char[] trimChars)
    {
        Value = Value ?? "";
        return Value.TrimStart(trimChars);
    }
}


//:Мусорка

//private static string[,] FormatStringForRewrite(string String, int count = 0)
//{
//	int start, end, i=0;
//	string tempString, endString;
//	if (count == 0) throw new NotImplementedException(); //Пусть ищет количество "@{" в строке
//	string[] outStrings = new string[count];
//	string[] outcolors = new string[count];

//	while (true)
//	{
//		start = String.IndexOf("@{");
//		if (start<=0) break;
//		tempString = String.Substring(start);
//		//do TODO: добавить исключающий символ
//		//{
//		//	end = tempString.IndexOf("}");
//		//	if (tempString[end-1]!= '\\') break;
//		//	endString = tempString.Substring(end);
//		//}
//		end = tempString.IndexOf(";");

//	} 
//	return  outStrings
//}

//public class Input {
//	public static string Uint = "12345678790";
//	public static string Int = "12345678790-";
//	public static string Udouble = "12345678790,";
//	public static string Double = "12345678790,-";
//}