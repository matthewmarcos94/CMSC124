﻿using System;
using Gtk;
using uLOLCODEv2;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;

namespace uLOLCODEv2
{
	public class ComplexEvaluator
	{
		public ComplexEvaluator ()
		{	
		}

		public String evaluateComplexExpression(String exp, TextView consoleText, Hashtable symbolTable) {
			Match m;
			String expression = exp;
			//List<String> operations = new List<String>();
			Stack stack = new Stack ();

			//consoleText.Buffer.Text += expression + "\n";

			while (!String.IsNullOrEmpty(expression)) {
				//consoleText.Buffer.Text += expression + "\n";
				m = Regex.Match (expression, @"WIN$");
				if (m.Success) {
					expression = expression.Remove (m.Index, m.Value.Length);
					expression = expression.Trim ();
					stack.Push (true);
					continue;
				}

				m = Regex.Match (expression, @"FAIL$");
				if (m.Success) {
					expression = expression.Remove (m.Index, m.Value.Length);
					expression = expression.Trim ();
					stack.Push (false);
					continue;
				}

				m = Regex.Match (expression, @"[a-zA-Z][a-zA-z\d]*$");
				if (m.Success && symbolTable.ContainsKey(m.Value)) {
					expression = expression.Remove (m.Index, m.Value.Length);
					expression = expression.Trim ();
					consoleText.Buffer.Text += m.Value + " found!\n";

					//Pushing variables.
					String myValue = symbolTable[m.Value].ToString();
					int number;
					Boolean isNumeric = int.TryParse(myValue, out number);

					//stack.Push (5);

					m = Regex.Match (myValue, @"\s*"".*""$");

					if (isNumeric) { //Numbr 
						stack.Push (number);
					} else if (myValue.Equals ("WIN")) { //Troof 
						stack.Push (true);						
					} else if (myValue.Equals ("FAIL")) { //Troof 
						stack.Push (false);
					} else if (m.Success) { //Yarn 
						stack.Push (m.Value);
					} 

					continue;
				}

				m = Regex.Match (expression, @"\-?\d+\s*$");
				if (m.Success) {
					expression = expression.Remove (m.Index, m.Value.Length);
					expression = expression.Trim ();
					stack.Push (Int32.Parse(m.Value));
					continue;
				}


				m = Regex.Match (expression, @"AN$");
				if (m.Success) {
					expression = expression.Remove (m.Index, m.Value.Length);
					expression = expression.Trim ();
					//consoleText.Buffer.Text += expression + "\n";
					continue;
				}

				m = Regex.Match (expression, @"SUM\s+OF$");
				if (m.Success) {
					expression = expression.Remove (m.Index, m.Value.Length);
					expression = expression.Trim ();
					var a = (int)stack.Pop ();
					var b = (int)stack.Pop ();
					stack.Push (a + b);
					continue;
				}

				m = Regex.Match (expression, @"DIFF\s+OF$");
				if (m.Success) {
					expression = expression.Remove (m.Index, m.Value.Length);
					expression = expression.Trim ();
					var a = (int)stack.Pop ();
					var b = (int)stack.Pop ();
					stack.Push (a - b);
					continue;
				}

				m = Regex.Match (expression, @"PRODUKT\s+OF$");
				if (m.Success) {
					expression = expression.Remove (m.Index, m.Value.Length);
					expression = expression.Trim ();
					var a = (int)stack.Pop ();
					var b = (int)stack.Pop ();
					stack.Push (a * b);
					continue;
				}

				m = Regex.Match (expression, @"QUOSHUNT\s+OF$");
				if (m.Success) {
					expression = expression.Remove (m.Index, m.Value.Length);
					expression = expression.Trim ();
					var a = (int)stack.Pop ();
					var b = (int)stack.Pop ();
					if (b == 0) {
						return "UNDEFINED!";
					}
					stack.Push (a / b);
					continue;
				}

				m = Regex.Match (expression, @"MOD\s+OF$");
				if (m.Success) {
					expression = expression.Remove (m.Index, m.Value.Length);
					expression = expression.Trim ();
					var a = (int)stack.Pop ();
					var b = (int)stack.Pop ();
					if (b == 0) {
						return "UNDEFINED!";
					}
					stack.Push (a % b);
					continue;
				}

				m = Regex.Match (expression, @"BIGGR\s+OF$");
				if (m.Success) {
					expression = expression.Remove (m.Index, m.Value.Length);
					expression = expression.Trim ();
					var a = (int)stack.Pop ();
					var b = (int)stack.Pop ();

					stack.Push ((a > b) ? a : b);
					continue;
				}

				m = Regex.Match (expression, @"SMALLR\s+OF$");
				if (m.Success) {
					expression = expression.Remove (m.Index, m.Value.Length);
					expression = expression.Trim ();
					var a = (int)stack.Pop ();
					var b = (int)stack.Pop ();

					stack.Push ((a > b) ? b : a);
					continue;
				}

				m = Regex.Match (expression, @"WON\s+OF\s+OF$");
				if (m.Success) {
					expression = expression.Remove (m.Index, m.Value.Length);
					expression = expression.Trim ();
					var a = (Boolean)stack.Pop ();
					var b = (Boolean)stack.Pop ();

					stack.Push ((a || b) && !(a && b));
					continue;
				}

				m = Regex.Match (expression, @"EITHER\s+OF$");
				if (m.Success) {
					expression = expression.Remove (m.Index, m.Value.Length);
					expression = expression.Trim ();
					var a = (Boolean)stack.Pop ();
					var b = (Boolean)stack.Pop ();

					stack.Push (a || b);
					continue;
				}

				m = Regex.Match (expression, @"BOTH\s+OF$");
				if (m.Success) {
					expression = expression.Remove (m.Index, m.Value.Length);
					expression = expression.Trim ();
					var a = (Boolean)stack.Pop ();
					var b = (Boolean)stack.Pop ();

					stack.Push (a && b);
					continue;
				}

			


				break;
			}

			String result = stack.Pop ().ToString();
			if (result.Equals ("True")) {
				return "WIN";
			} else if (result.Equals ("False")) {
				return "FAIL";
			} else {
				return result;
			}
			//return stack.Pop ().ToString();
		}



	}
}

