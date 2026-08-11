using System.Text.RegularExpressions;
using System.Windows; using System.Windows.Controls; using System.Windows.Input;
namespace LeaziEnergiaSolar.Wpf.Utils;
public enum InputType { Livre, SomenteNumeros, SomenteLetras, LetrasENumeros, Decimal }
public static class InputBehavior {
 public static readonly DependencyProperty TypeProperty=DependencyProperty.RegisterAttached("Type",typeof(InputType),typeof(InputBehavior),new PropertyMetadata(InputType.Livre,Changed));
 public static void SetType(DependencyObject e,InputType v)=>e.SetValue(TypeProperty,v); public static InputType GetType(DependencyObject e)=>(InputType)e.GetValue(TypeProperty);
 static void Changed(DependencyObject d,DependencyPropertyChangedEventArgs e){if(d is not TextBox t)return;t.PreviewTextInput-=Input;DataObject.RemovePastingHandler(t,Paste);if((InputType)e.NewValue==InputType.Livre)return;t.PreviewTextInput+=Input;DataObject.AddPastingHandler(t,Paste);}
 static void Input(object s,TextCompositionEventArgs e){var t=(TextBox)s;e.Handled=!Valid(GetType(t),Prospective(t,e.Text));}
 static void Paste(object s,DataObjectPastingEventArgs e){var t=(TextBox)s;var value=e.SourceDataObject.GetData(DataFormats.Text) as string??"";if(!Valid(GetType(t),Prospective(t,value)))e.CancelCommand();}
 static string Prospective(TextBox t,string input){var value=t.Text??"";if(t.SelectionLength>0)value=value.Remove(t.SelectionStart,t.SelectionLength);return value.Insert(t.SelectionStart,input);}
 static bool Valid(InputType type,string value)=>string.IsNullOrEmpty(value)||type switch { InputType.SomenteNumeros=>Regex.IsMatch(value,@"^\d+$"),InputType.SomenteLetras=>Regex.IsMatch(value,@"^[\p{L}\s]+$"),InputType.LetrasENumeros=>Regex.IsMatch(value,@"^[\p{L}\d\s]+$"),InputType.Decimal=>Regex.IsMatch(value,@"^\d*([,.]\d{0,2})?$"),_=>true};
}
