Imports Microsoft.PointOfService

Public Class cESCPOS
    Public Const eClear As String = Chr(27) + "@"
    Public Const eCentre As String = Chr(27) + Chr(97) + "1"
    Public Const eLeft As String = Chr(27) + Chr(97) + "0"
    Public Const eRight As String = Chr(27) + Chr(97) + "2"
    'Public Const eDrawer As String = eClear + Chr(27) + "p" + Chr(0) + ".}"
    Public Const eDrawer As String = eClear + Chr(27) + "p" + Chr(0) + "n}"
    Public Const eDrawer2 As String = eClear + Chr(27) + "p" + Chr(1) + "n}"
    Public Const eCut As String = Chr(27) + "i" + vbCrLf
    Public Const eSmlText As String = Chr(27) + "!" + Chr(1)
    Public Const eNmlText As String = Chr(27) + "!" + Chr(0)
    Public Const eInit As String = eNmlText + Chr(13) + Chr(27) + "c6" + Chr(1) + Chr(27) + "R3" + vbCrLf
    Public Const eBigCharOn As String = Chr(27) + "!" + Chr(56)
    Public Const eBigCharOff As String = Chr(27) + "!" + Chr(0)
    Public Const eBoldOn As String = Chr(27) + "E1"
    Public Const eBoldOff As String = Chr(27) + "E0"
    Public Const eLeftMargin As String = Chr(27) + "l"
    Public Const eSelectColor2 As String = Chr(27) + Chr(114) + "1"
    'ESC r n
    '   27 114 n
    'When n = 0,48, color 1 Is selected.
    'When n = 1,49, color 2 Is selected.


    'Prints n lines of blank paper.
    Public Shared Function feed(iFeed As Integer) As String
        Return Chr(27) & "d" & iFeed.ToString
    End Function

    Public Shared Function setBold(bool As Boolean) As String
        Dim sData As String = Chr(27) & "E"

        If (bool) Then
            sData = sData & "1"
        Else
            sData = sData & "0"
        End If

        Return sData

    End Function

    Public Shared Function setInverse(bool As Boolean) As String
        Dim sData As String = Chr(29) & "B"
        If (bool) Then
            sData = sData & "1"
        Else
            sData = sData & "0"
        End If

        Return sData

    End Function

    '0 = no underline.
    '1 = single weight underline.
    '2 = double weight underline.
    Public Shared Function setUnderline(val As Integer) As String
        Return Chr(27) & "-" & val.ToString
    End Function

    Public Shared Function setFontSize(val As Integer) As String
        If (val > 3) Then
            Return Chr(27) & "!" & Chr(val)
        Else
            Return Chr(27) & "!" & Chr(0)
        End If

    End Function

    '0 = left justify.
    '1 = center justify.
    '2 = right justify.
    Public Shared Function setJustification(val As Integer) As String
        Return Chr(27) & "a" & val.ToString
    End Function

    Public Shared Function setLineSpacing(spacing As Integer) As String
        Return Chr(27) & "3" & spacing.ToString
    End Function

    Public Shared Function cut() As String
        Return Chr(29) & "V" & Chr(48) & Chr(0)
    End Function

    Public Shared Function beep(beeperTimes As Integer) As String

        '[Name] [Format]
        '[Range] [Description]
        'Printer Beeper And Alarm Notification [For GP-80250I] ASCII ESC C m t n
        'Hex 1B 43 m t n
        'Decimal 27 67 m t n 1<=m<=20, 1 <= t <= 20, 0 <= n <= 3
        'Beeper And alarm functions when receipt comes.
        '• m: 1<=m<=20, the functions times of beeper And alarm.
        '• t: 1<=t<=20, the beeper And alarm lasting time, which Is [tx50]ms. • When n=0, beeper And alarm are disabled
        'When n=1, beeper Is enabled
        'When n=2, alarm Is enabled
        'When n=3, beeper And alarm are enabled

        Return Chr(27) & "B" & Chr(beeperTimes) & Chr(9) & Chr(1)

    End Function

    Public Shared Function setMarginLeft(val As Integer) As String
        Return eLeftMargin & val.ToString
    End Function
    '    /**
    '	 * Encode And print QR code
    '	 * 
    '	 * @param str
    '	 *          String to be encoded in QR.
    '	 * @param errCorrection
    '	 *          The degree of error correction. (48 <= n <= 51)
    '	 *          48 = level L / 7% recovery capacity.
    '	 *          49 = level M / 15% recovery capacity.
    '	 *          50 = level Q / 25% recovery capacity.
    '	 *          51 = level H / 30% recovery capacity.
    '	 *          
    '	 *  @param moduleSize
    '	 *  		The size of the QR module (pixel) in dots.
    '	 *  		The QR code will Not print if it Is too big.
    '	 *  		Try setting this low And experiment in making it larger.
    '	 */
    '	Public void printQR(String str, int errCorrect, int moduleSize){
    '		//save data function 80
    '		printer.write(0x1D);//init
    '		printer.write("(k");//adjust height of barcode
    '		printer.write(str.length()+3); //pl
    '		printer.write(0); //ph
    '		printer.write(49); //cn
    '		printer.write(80); //fn
    '		printer.write(48); //
    '		printer.write(str);

    '		  //error correction function 69
    '		printer.write(0x1D);
    '		printer.write("(k");
    '		printer.write(3); //pl
    '		printer.write(0); //ph
    '		printer.write(49); //cn
    '		printer.write(69); //fn
    '		printer.write(errCorrect); //48<= n <= 51

    '		  //size function 67
    '		printer.write(0x1D);
    '		printer.write("(k");
    '		printer.write(3);
    '		printer.write(0);
    '		printer.write(49);
    '		printer.write(67);
    '		printer.write(moduleSize);//1<= n <= 16

    '		  //print function 81
    '		printer.write(0x1D);
    '		printer.write("(k");
    '		printer.write(3); //pl
    '		printer.write(0); //ph
    '		printer.write(49); //cn
    '		printer.write(81); //fn
    '		printer.write(48); //m   
    '}



    '    /**
    '	 * Encode And print barcode
    '	 * 
    '	 * @param code
    '	 *          String to be encoded in the barcode. 
    '	 *          Different barcodes have different requirements on the length
    '	 *          of data that can be encoded.
    '	 * @param type
    '	 *          Specify the type of barcode
    '	 *          65 = UPC-A.
    '	 *          66 = UPC-E.
    '	 *          67 = JAN13(EAN).
    '	 *          68 = JAN8(EAN).
    '	 *          69 = CODE39.
    '	 *          70 = ITF.
    '	 *          71 = CODABAR.
    '	 *          72 = CODE93.
    '	 *          73 = CODE128.
    '	 *          
    '	 *  @param h
    '	 *  		height of the barcode in points (1 <= n <= 255)
    '	 *  @param w
    '	 *  		width of module (2 <= n <=6).
    '	 *  		Barcode will Not print if this value Is too large.
    '	 *  @param font
    '	 *  		Set font of HRI characters
    '	 *  		0 = font A
    '	 *  		1 = font B
    '	 *  @param pos
    '	 *  		set position of HRI characters
    '	 *  		0 = Not printed.
    '	 *  		1 = Above barcode.
    '	 *  		2 = Below barcode.
    '	 *  		3 = Both above And below barcode.
    '	 */
    '	Public void printBarcode(String code, int type, int h, int w, int font, int pos){

    '		//need to test for errors in length of code
    '		//also control for input type=0-6

    '		//GS H = HRI position
    '		printer.write(0x1D);
    '		printer.write("H");
    '		printer.write(pos); //0=no print, 1=above, 2=below, 3=above & below

    '		//GS f = set barcode characters
    '		printer.write(0x1D);
    '		printer.write("f");
    '		printer.write(font);

    '		//GS h = sets barcode height
    '		printer.write(0x1D);
    '		printer.write("h");
    '		printer.write(h);

    '		//GS w = sets barcode width
    '		printer.write(0x1D);
    '		printer.write("w");
    '		printer.write(w);//module = 1-6

    '		//GS k
    '		printer.write(0x1D); //GS
    '		printer.write("k"); //k
    '		printer.write(type);//m = barcode type 0-6
    '		printer.write(code.length()); //length of encoded string
    '		printer.write(code);//d1-dk
    '		printer.write(0);//print barcode
    '}

    '    /**
    '	 * Encode And print PDF 417 barcode
    '	 * 
    '	 * @param code
    '	 *          String to be encoded in the barcode. 
    '	 *          Different barcodes have different requirements on the length
    '	 *          of data that can be encoded.
    '	 * @param type
    '	 *          Specify the type of barcode
    '	 *          0 - Standard PDF417
    '	 *          1 - Standard PDF417
    '	 *          
    '	 *  @param h
    '	 *  		Height of the vertical module in dots 2 <= n <= 8.
    '	 *  @param w
    '	 *  		Height of the horizontal module in dots 1 <= n <= 4.
    '	 *  @param cols
    '	 *  		Number of columns 0 <= n <= 30.
    '	 *  @param rows
    '	 *  		Number of rows 0 (automatic), 3 <= n <= 90.
    '	 *  @param error
    '	 *  		set error correction level 48 <= n <= 56 (0 - 8).
    '	 *  		
    '	 */
    '	Public void printPSDCode(String code, int type, int h, int w, int cols, int rows, int Error){	

    '	 //print function 82
    '	  printer.write(0x1D);
    '	  printer.write("(k");
    '	  printer.write(code.length()); //pl Code length
    '	  printer.write(0); //ph
    '	  printer.write(48); //cn
    '	  printer.write(80); //fn
    '	  printer.write(48); //m
    '	  printer.write(code); //data to be encoded


    '	  //function 65 specifies the number of columns
    '	  printer.write(0x1D);//init
    '	  printer.write("(k");//adjust height of barcode
    '	  printer.write(3); //pl
    '	  printer.write(0); //pH
    '	  printer.write(48); //cn
    '	  printer.write(65); //fn
    '	  printer.write(cols);

    '	  //function 66 number of rows
    '	  printer.write(0x1D);//init
    '	  printer.write("(k");//adjust height of barcode
    '	  printer.write(3); //pl
    '	  printer.write(0); //pH
    '	  printer.write(48); //cn
    '	  printer.write(66); //fn 
    '	  printer.write(rows); //num rows

    '	  //module width function 67
    '	  printer.write(0x1D);
    '	  printer.write("(k");
    '	  printer.write(3);//pL
    '	  printer.write(0);//pH
    '	  printer.write(48);//cn
    '	  printer.write(67);//fn
    '	  printer.write(w);//size of module 1<= n <= 4

    '	  //module height fx 68
    '	  printer.write(0x1D);
    '	  printer.write("(k");
    '	  printer.write(3);//pL
    '	  printer.write(0);//pH
    '	  printer.write(48);//cn
    '	  printer.write(68);//fn
    '	  printer.write(h);//size of module 2 <= n <= 8

    '	  //error correction function 69
    '	  printer.write(0x1D);
    '	  printer.write("(k");
    '	  printer.write(4);//pL
    '	  printer.write(0);//pH
    '	  printer.write(48);//cn
    '	  printer.write(69);//fn
    '	  printer.write(48);//m
    '	  printer.write(error);//error correction

    '	  //choose pdf417 type function 70
    '	  printer.write(0x1D);
    '	  printer.write("(k");
    '	  printer.write(3);//pL
    '	  printer.write(0);//pH
    '	  printer.write(48);//cn
    '	  printer.write(70);//fn
    '	  printer.write(type);//set mode of pdf 0 Or 1

    '	  //print function 81
    '	  printer.write(0x1D);
    '	  printer.write("(k");
    '	  printer.write(3); //pl
    '	  printer.write(0); //ph
    '	  printer.write(48); //cn
    '	  printer.write(81); //fn
    '	  printer.write(48); //m

    '}

End Class
