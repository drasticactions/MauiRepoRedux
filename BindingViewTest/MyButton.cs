using System.ComponentModel;

namespace BindingViewTest;

[DefaultProperty( "Content" )]
[ContentProperty( "Content" )]
public class MyButton : TemplatedView
{
    public static readonly BindableProperty ContentProperty = BindableProperty.Create( nameof( Content ), typeof( View ), typeof( MyButton ) );

    public View Content
    {
        get => (View)GetValue( ContentProperty );
        set => SetValue( ContentProperty, value );
    }
}