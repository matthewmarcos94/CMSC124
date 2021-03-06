/*
 * Created by: Mon, Matt, and James
 * 
 */
using System;
using Gtk;
using uLOLCODEv2;
using System.Collections;
using System.Text.RegularExpressions;
using System.Collections.Generic;

public partial class MainWindow: Gtk.Window
{		
	public static Gtk.ListStore lexModel = new Gtk.ListStore(typeof(string),typeof(string));
	public static Gtk.ListStore symbolTree = new Gtk.ListStore(typeof(string),typeof(string));
	public static Hashtable symbolTable = new Hashtable ();
	//O RLY - OIC Needs
	public static Boolean detectedOIC = false;
	public static int newIndex = 0;

//	Identifier ident = new Identifier();
	EvalClass eval = new EvalClass ();
	LexerClass lexer = new LexerClass();
	ParserClass parser = new ParserClass ();
	//TestClass test = new TestClass();

	public MainWindow (): base (Gtk.WindowType.Toplevel)
	{	
		Build ();
		populateTreeView ();
		inputCode.Buffer.Text = ("HAI\n\nKTHXBYE");
	}

	public void populateTreeView(){
		//instantiate 2 columns
		Gtk.TreeViewColumn lexemeCol = new Gtk.TreeViewColumn ();
		lexemeCol.Title = "Lexeme";

		Gtk.TreeViewColumn classificationCol = new Gtk.TreeViewColumn ();
		classificationCol.Title = "Classification";

		//add the 2 columns to the treeview
		treeview1.AppendColumn (lexemeCol);
		treeview1.AppendColumn (classificationCol);

		treeview1.Model = lexModel;
		Gtk.CellRendererText lexemeCell = new Gtk.CellRendererText ();
		lexemeCol.PackStart (lexemeCell, true);
		Gtk.CellRendererText classCell = new Gtk.CellRendererText ();
		classificationCol.PackStart (classCell, true);

		lexemeCol.AddAttribute (lexemeCell, "text", 0);
		classificationCol.AddAttribute (classCell, "text", 1);

		//===================================================//

		Gtk.TreeViewColumn identifierCol = new Gtk.TreeViewColumn ();
		identifierCol.Title = "Identifier";

		Gtk.TreeViewColumn valueCol = new Gtk.TreeViewColumn ();
		valueCol.Title = "Value";

		//add the 2 columns to the treeview
		treeview2.AppendColumn (identifierCol);
		treeview2.AppendColumn (valueCol);

		treeview2.Model = symbolTree;
		Gtk.CellRendererText identifierCell = new Gtk.CellRendererText ();
		identifierCol.PackStart (identifierCell, true);
		Gtk.CellRendererText valueCell = new Gtk.CellRendererText ();
		valueCol.PackStart (valueCell, true);

		identifierCol.AddAttribute (identifierCell, "text", 0);
		valueCol.AddAttribute (valueCell, "text", 1);
	}

	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		Application.Quit ();
		a.RetVal = true;
	}

	protected void executeCode (object sender, EventArgs e)
	{
		consoleText.Buffer.Text = "";
		String code = inputCode.Buffer.Text;
		Boolean isComment = false;

		lexModel.Clear ();
		symbolTree.Clear ();
		symbolTable.Clear ();

		char[] splitToken = {'\n'};
		String[] lines = code.Split (splitToken);
		symbolTable.Add("IT","NOOB");
		if(!eval.hasValidStartAndEnd(lines)) {
			consoleText.Buffer.Text += "Syntax Error at program delimiter\n";
		} 

		for(var i = 0 ; i < lines.Length ; i++) {
			lexer.parseLines(lines[i], ref isComment, i, ref lexModel, ref symbolTree, consoleText);
		}

		for(var i = 0 ; i < lines.Length ; i++) {
			if(detectedOIC){
				i = newIndex;
				detectedOIC = false;
			}
			parser.parseLines(lines[i], ref isComment, i, ref symbolTable, ref symbolTree, consoleText,lines);
		}

	}
		
	protected void updateSymbolTable() {
		symbolTree.Clear ();
		foreach(DictionaryEntry pair in symbolTable) {
			symbolTree.AppendValues (pair.Key, pair.Value);
		}
	}


	protected void openCODE (object sender, EventArgs e)
	{
		FileChooserDialog fileOpen = new FileChooserDialog ("Open File",this,FileChooserAction.Open,
		                                                    "Cancel", ResponseType.Cancel,"Open", ResponseType.Accept);		//for open button 
		//fileOpen.AddFilter ("LOLCODE (.lol)|*.lol|All Files (*.*)|*.*"); 
		if( fileOpen.Run() == (int)ResponseType.Accept){
			//open file for reading
			System.IO.StreamReader file = System.IO.File.OpenText(fileOpen.Filename);

			//display in the textbox
			inputCode.Buffer.Text = file.ReadToEnd();

		}
		fileOpen.Destroy();
	}
}
