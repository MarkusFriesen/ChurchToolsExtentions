using ChurchToolsExtentions;

namespace ChurchToolsExtentionsTests;

public class FileFormatterTests
{
    string content = @"#LangCount=1
#Title=So sehr hat Gott die Welt Geliebt
#Version=3
#Format=F/K//
#TitleFormat=U
#(c)=Public Domain
#VerseOrder=Verse 1,Chorus,Verse 1
---
Verse 1
So sehr hat Gott diese Welt geliebt, 
dass es seinen eignen Sohn für uns hingab. 
Damit jeder der an den Christus glaubt 
nicht verloren geht und in Ewigkeit lebt.
nicht verloren geht und in Ewigkeit lebt.
---
Chorus
Halleluja, Halleluja, Halleluja, Halleluja. Amen!
Halleluja, Halleluja, Halleluja, Amen.
";

    [Fact]
    public void NullSplitterReturnsContent()
    {
        var formatter = new FileFormatter(new() { });

        var result = formatter.Format(content);
        Assert.Equal(content, result);
    }

    [Fact]
    public void SplitLines()
    {
        var formatter = new FileFormatter(new() { MaxNumberOfLines = 2 });

        var result = formatter.Format(content);
        Assert.NotEqual(content, result);

        var expected = @"#LangCount=1
#Title=So sehr hat Gott die Welt Geliebt
#Version=3
#Format=F/K//
#TitleFormat=U
#(c)=Public Domain
#VerseOrder=Verse 1,Chorus,Verse 1
---
Verse 1
So sehr hat Gott diese Welt geliebt, 
dass es seinen eignen Sohn für uns hingab. 
---
Damit jeder der an den Christus glaubt 
nicht verloren geht und in Ewigkeit lebt.
---
nicht verloren geht und in Ewigkeit lebt.
---
Chorus
Halleluja, Halleluja, Halleluja, Halleluja. Amen!
Halleluja, Halleluja, Halleluja, Amen.";
        Assert.Equal(expected, result);
    }
}
