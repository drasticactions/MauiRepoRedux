using Android.Text;

namespace AndroidTest;

[Activity(Label = "@string/app_name", MainLauncher = true)]
public class MainActivity : Activity
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        // Set our view from the "main" layout resource
        SetContentView(Resource.Layout.activity_main);

        var htmlCodeText = @"<h2>test</h2><code><label for=""name"">Name (4 to 8 characters):</label><input type=""text"" id=""name"" name=""name"" required minlength=""4"" maxlength=""8"" size=""10"" /></code>";
        
        // Find the TextView in the layout
        var textView = FindViewById<TextView>(Resource.Id.textView);

        // Set the HTML content to the TextView

        textView.SetText(Html.FromHtml(htmlCodeText), TextView.BufferType.Spannable);
    }
}