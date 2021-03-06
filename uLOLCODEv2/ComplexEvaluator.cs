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

		public String removeQuotes(String expression) {
			return expression.Substring (1, expression.Length - 2);
		}

		public String evaluateComplexExpression(String exp, TextView consoleText, Hashtable symbolTable) {
			Match m;
			String expression = exp;
			Stack stack = new Stack ();
			int[] mkTier;
			mkTier = new int [225];
			int currTier = 0;
			for (int i = 0; i < 225; i++) {
				mkTier [i] = 0;
			}

			while (!String.IsNullOrEmpty (expression)) {
				//Win literal
				m = Regex.Match (expression, @"WIN$");
				if (m.Success) {
					expression = expression.Remove (m.Index, m.Value.Length);
					expression = expression.Trim ();
					stack.Push (true);
					if (currTier > 0) {
						mkTier[currTier]++;
					}
					continue;
				}

				//Fail literal
				m = Regex.Match (expression, @"FAIL$");
				if (m.Success) {
					expression = expression.Remove (m.Index, m.Value.Length);
					expression = expression.Trim ();
					stack.Push (false);
					if (currTier > 0) {
						mkTier[currTier]++;
					}
					continue;
				} 	

				//Variable Identifier
				m = Regex.Match (expression, @"[a-zA-Z][a-zA-z\d]*$");
				if (m.Success && symbolTable.ContainsKey(m.Value)) {
					expression = expression.Remove (m.Index, m.Value.Length);
					expression = expression.Trim ();

					//Pushing variables.
					String myValue = symbolTable[m.Value].ToString();
					//check if var stored is string
					m = Regex.Match (myValue, @"\s*"".*""$");
					if (myValue.Equals ("WIN")) { //Troof 						
						stack.Push (true);						
					} else if (myValue.Equals ("FAIL")) { //Troof 
						stack.Push (false);
					} else if (m.Success) { //Yarn 
						stack.Push (m.Value);
					} else if(Regex.IsMatch(myValue, @"\s*\d*\.?\d+\s*$")) {//Numbr || Numbar
						stack.Push (float.Parse(myValue));
					}

					if (currTier > 0) {
						mkTier[currTier]++;
					}
					continue;
				}

				//String Literal
				m = Regex.Match (expression, @"""([^""]*)""$");
				if (m.Success) {
					expression = expression.Remove (m.Index, m.Value.Length);
					expression = expression.Trim ();
					stack.Push (m.Value);
					if (currTier > 0) {
						mkTier[currTier]++;
					}
					continue;
				}

				//Numbr || Numbar Literal
				m = Regex.Match (expression, @"\-?\d*\.?\d+\s*$");
				if (m.Success) {
					expression = expression.Remove (m.Index, m.Value.Length);
					expression = expression.Trim ();

					stack.Push (float.Parse (m.Value));

					if (currTier > 0) {
						mkTier[currTier]++;
					}
					continue;
				}

				m = Regex.Match (expression, @"AN$");
				if (m.Success) {
					expression = expression.Remove (m.Index, m.Value.Length);
					expression = expression.Trim ();
					continue;
				}

				m = Regex.Match (expression, @"SUM\s+OF$");
				if (m.Success) {
					expression = expression.Remove (m.Index, m.Value.Length);
					expression = expression.Trim ();
					var a = stack.Pop ().ToString();
					var b = stack.Pop ().ToString();

					float op1 = float.Parse(a);
					float op2 = float.Parse(b);
					stack.Push (op1 + op2);

					if (currTier > 0) {
						mkTier[currTier]--;
					}
					continue;
				}

				m = Regex.Match (expression, @"DIFF\s+OF$");
				if (m.Success) {
					expression = expression.Remove (m.Index, m.Value.Length);
					expression = expression.Trim ();
					var a = stack.Pop ().ToString();
					var b = stack.Pop ().ToString();

					float op1 = float.Parse(a);
					float op2 = float.Parse(b);
					stack.Push (op1 - op2);

					if (currTier > 0) {
						mkTier[currTier]--;
					}
					continue;
				}

				m = Regex.Match (expression, @"PRODUKT\s+OF$");
				if (m.Success) {
					expression = expression.Remove (m.Index, m.Value.Length);
					expression = expression.Trim ();
					var a = stack.Pop ().ToString();
					var b = stack.Pop ().ToString();

					float op1 = float.Parse(a);
					float op2 = float.Parse(b);
					stack.Push (op1 * op2);

					if (currTier > 0) {
						mkTier[currTier]--;
					}
					continue;
				}

				m = Regex.Match (expression, @"QUOSHUNT\s+OF$");
				if (m.Success) {
					expression = expression.Remove (m.Index, m.Value.Length);
					expression = expression.Trim ();

					var a = stack.Pop ().ToString();
					var b = stack.Pop ().ToString();

					if (b.Equals("0")) {
						return "UNDEFINED";
					}
					float op1 = float.Parse(a);
					float op2 = float.Parse(b);
					stack.Push (op1 / op2);

					if (currTier > 0) {
						mkTier[currTier]--;
					}
					continue;
				}

				m = Regex.Match (expression, @"MOD\s+OF$");
				if (m.Success) {
					expression = expression.Remove (m.Index, m.Value.Length);
					expression = expression.Trim ();
					var a = stack.Pop ().ToString();
					var b = stack.Pop ().ToString();

					if (b.Equals("0")) {
						return "UNDEFINED";
					}
					float op1 = float.Parse(a);
					float op2 = float.Parse(b);
					stack.Push (op1 % op2);

					if (currTier > 0) {
						mkTier[currTier]--;
					}
					continue;
				}

				m = Regex.Match (expression, @"BIGGR\s+OF$");
				if (m.Success) {
					expression = expression.Remove (m.Index, m.Value.Length);
					expression = expression.Trim ();

					var a = stack.Pop ().ToString();
					var b = stack.Pop ().ToString();

					float op1 = float.Parse(a);
					float op2 = float.Parse(b);
					stack.Push ((op1 > op2) ? op1 : op2);
					
					if (currTier > 0) {
						mkTier[currTier]--;
					}
					continue;
				}

				m = Regex.Match (expression, @"SMALLR\s+OF$");
				if (m.Success) {
					expression = expression.Remove (m.Index, m.Value.Length);
					expression = expression.Trim ();
					var a = stack.Pop ().ToString();
					var b = stack.Pop ().ToString();

					float op1 = float.Parse(a);
					float op2 = float.Parse(b);
					stack.Push ((op1 > op2) ? op2 : op1);

					if (currTier > 0) {
						mkTier[currTier]--;
					}
					continue;
				}

				m = Regex.Match (expression, @"NOT$");
				if (m.Success) {
					expression = expression.Remove (m.Index, m.Value.Length);
					expression = expression.Trim ();
					var a = (Boolean)stack.Pop ();

					stack.Push (!a);
					continue;
				}

				m = Regex.Match (expression, @"WON\s+OF$");
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

				m = Regex.Match (expression, @"BOTH\s+SAEM$");
				if (m.Success) {
					expression = expression.Remove (m.Index, m.Value.Length);
					expression = expression.Trim ();
					var a = stack.Pop ().ToString();
					var b = stack.Pop ().ToString();

					stack.Push (a.Equals(b));
					continue;
				}

				m = Regex.Match (expression, @"DIFFRINT$");
				if (m.Success) {
					expression = expression.Remove (m.Index, m.Value.Length);
					expression = expression.Trim ();
					var a = stack.Pop ().ToString();
					var b = stack.Pop ().ToString();

					stack.Push (a != b);
					continue;
				}


				m = Regex.Match (expression, @"SMOOSH$");
				if (m.Success) {
					if (currTier <= 0) {
						return "UNDEFINED";
					}
					expression = expression.Remove (m.Index, m.Value.Length);
					expression = expression.Trim ();
					List<String> strings = new List<String> ();
					String tempString = "";

					for (var i = 0; i < mkTier[currTier]; i++) {
						var a = stack.Pop().ToString();

						if (a.Equals ("True") || a.Equals ("False")) {
							return "UNDEFINED";
						}

						if (Regex.IsMatch (a, @"\s*"".*""$")) {
							
							a = removeQuotes (a);
						}

						strings.Add (a);
					}

					for (var i = 0; i < mkTier[currTier] ; i++) {
						tempString += strings [i];
					}
					stack.Push ("\"" + tempString + "\"");
					currTier--;
					continue; 
				}

				//All OF
				m = Regex.Match (expression, @"ALL\s+OF$");
				if (m.Success) {
					if (currTier <= 0) {
						return "UNDEFINED";
					}

					expression = expression.Remove (m.Index, m.Value.Length);
					expression = expression.Trim ();
					List<Boolean> bools = new List<Boolean> ();
					Boolean answer = true;

					for (var i = 0; i < mkTier[currTier]; i++) {
						var a = stack.Pop().ToString();

						if (a.Equals ("True")) {
							bools.Add (true);
						} else if (a.Equals ("False")) {
							bools.Add (false);
						} else {
							return "UNDEFINED";
						}
					}

					for (var i = 0; i < mkTier [currTier]; i++) {
						answer = answer && bools [i];					
					}
					stack.Push (answer);
					currTier--;
					continue; 
				}
					
				m = Regex.Match (expression, @"ANY\s+OF$");
				if (m.Success) {
					if (currTier <= 0) {
						return "UNDEFINED";
					}

					expression = expression.Remove (m.Index, m.Value.Length);
					expression = expression.Trim ();
					List<Boolean> bools = new List<Boolean> ();
					Boolean answer = false;

					for (var i = 0; i < mkTier[currTier]; i++) {
						var a = stack.Pop().ToString();

						if (a.Equals ("True")) {
							bools.Add (true);
						} else if (a.Equals ("False")) {
							bools.Add (false);
						} else {
							return "UNDEFINED";
						}
					}

					for (var i = 0; i < mkTier [currTier]; i++) {
						answer = answer || bools [i];					
					}
					stack.Push (answer);
					currTier--;
					continue; 
				}


				m = Regex.Match (expression, @"\s+MKAY$");
				if (m.Success) {
					expression = expression.Remove (m.Index, m.Value.Length);
					expression = expression.Trim ();
					currTier++;
					continue;
				}

				return "UNDEFINED";
			}//End of main loop

			String result = stack.Pop ().ToString();
			if (result.Equals ("True")) {
				return "WIN";
			} else if (result.Equals ("False")) {
				return "FAIL";
			} else {
				return result;
			}
		}
	}//End of ComplexEvaluatorClass
}

