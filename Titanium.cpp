#define _CRT_SECURE_NO_WARNINGS
#include <iostream>
#include <math.h>
#include <conio.h>
#include <Windows.h>
#include <string>
#include <fstream>

using namespace std;
#define minint -2147483647
#define ushort unsigned short
#define byte unsigned char


	enum {черн, тсин, тзел, тгол, крас, лил, жел, ссер, сер, син, зел, гол, роз, фиол, тел, бел };
	char default_background = черн, default_text_color = бел;

	HANDLE hConsole = GetStdHandle(STD_OUTPUT_HANDLE); //Для setconsole (изм цвета фона и текста)
	void clr(char background, char color) { SetConsoleTextAttribute(hConsole, (WORD)((background << 4) | color)); }
	void clr(char color) { SetConsoleTextAttribute(hConsole, (WORD)((default_background << 4) | color)); }
	void clr() { SetConsoleTextAttribute(hConsole, (WORD)((default_background << 4) | default_text_color)); }

	bool qu() { //Функция вопроса
		char* q = new char[31]; bool answer;
		clr(жел); printf(" ");
		do { fgets(q, 31, stdin); } while (q[0] == '\n');
		clr();
		if (((q[0] == 'д') || (q[0] == 'y') || (q[0] == '1') || (strstr(q, "конечно") != 0) || (strstr(q, "Sbrakets") != 0) || (strstr(q, "ок") != 0) || (strstr(q, "ладно") != 0) || (strstr(q, "угу") != 0) || (strstr(q, "ага") != 0) || (strstr(q, "хорошо") != 0) || (strstr(q, "хочу") != 0)) && (q[3] != 'н')) answer = true; else answer = false;
		delete[] q;
		return answer;
	}

	bool Isdigit(char c) {
		return (c >= '0' && c <= '9');

	}

	unsigned int getuint() {
		unsigned int val = 0;
		clr(жел);
		for (unsigned char i = 0;;)
		{
			i = _getch();
			if (i == '\r') { break; } //если Enter
			if (Isdigit(i)) { val = val * 10 + (i - 48); printf("%c", i); }
			if ((i == '\b')) { //Если нажат backspace
				if (val != 0) { val = floor(val / 10); printf("\b \b"); } //Если есть число
			}
		}
		clr();
		return val;
	}

	int getint() {
		int val = 0;
		clr(жел);
		for (unsigned char i = 0;;)
		{
			bool minus = false;
			i = _getch();
			if (i == '\r') { if (minus) val *= -1; break; }
			if (i == '-' && !minus) { printf("%c", i); minus = true; }
			if (Isdigit(i)) { val = val * 10 + (i - 48); printf("%c", i); }
			if ((i == '\b')) { //Если нажат backspace
				if (val != 0) { val = floor(val / 10); printf("\b \b"); } //Если есть число
				else if (minus) { minus = false; printf("\b \b"); }
			} //Если есть минус
		}
		clr();
		return val;
	}

	string getstring() { //Нормальная функция ввода строки, исключающая возможность ввести пустую строку + цвет
		string input = "";
		clr();
		char symbol;
		while (true) {
			symbol = _getch();

			if (symbol == '\r') if (input == "") continue; else break; //Завершить ввод при нажатии Enter только ЕСЛИ строка не пуста
			if (symbol == '\b') if (input != "") { printf("\b \b"); input.pop_back(); continue; }
			else continue;

			input += symbol; printf("%c", symbol);
			if (symbol == 27) return ("" + (char)(27));
		}
		clr();
		return input;
	}

	void perr(string massage) {
		clr(крас); cout << "\n" << massage; clr();
	}
