   M S B A M L   `   `   `  ���� C  ?location, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null#clr-namespace:locationlocation      location.ClientInterface   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     (localclr-namespace:location     -   $��window��  � Title$  Client��$��464.97��$��402.444�� � 
ResizeMode$ NoResize=� � 	Resources  �� 
TargetType �� Triggers P� Property P� Value �� Property �� Value	 �� 
TargetType��%5  '��       '�� �     �� " �� P�  �@d�	IsChecked$
 TrueY�. ���  � ��
 �  ��P�  �@d�	IsChecked$ FalseY�. ���  � ��
 ������� " ��.���  � �� �� "	 ��.R��� #��z��._��� $��Center=�$��Center=� P�  �@J�IsMouseOver . . ���  � ��
 � � �.��� $��	5,-40,5,5q�
 � ColumnDefinitions
 �� $��* ��� $��* ��� $��* � � RowDefinitions �� $��* ��� $��* ��� $��* ��� $��* ��� $��* ��� $��* ��� $��* ��� $��* �.+��� -    ��Name$ TimeOutTextBox�� �� TextWrapping$
 Wrap=�$��1��$��2��$��Center=�$��Center=�$��16(����� *	Timeout+ �� ElementName$ window�� �� Mode$ TwoWay=��� -   $ HostTextBox��$
 Wrap=�$��1��$��1��$��Center=�$��Center=�$��16(����� *Host+$ window��$ TwoWay=��� -   $ NameTextBox��$
 Wrap=�$��1��$��3��$��Center=�$��Center=�$��16(����� *
UserName+$ window��$ TwoWay=��� -   $ LocationTextBox��$
 Wrap=�$��1��$��4��$��Center=�$��Center=�$��16(����� *
Location+$ window��$ TwoWay=��� -   $ PortTextBox��$
 Wrap=�$��1��$��5��$��Center=�$��Center=�$��16(����� *Port+$ window��$ TwoWay=��� -   $ ServerResponseTextBox�� �� 
IsReadOnly . $
 Wrap=�$��2��$��1��$��7��$��16(����� *ServerResponse+$ window��$ OneWay=��� -    ��Name$ ProtocolsComboBox�� �� SelectedIndex$ 0��$��2��$��Center=�$��Center=�$��16(�$��1�� �� SelectedItem �� *
Protocol+$ window��$ TwoWay=�.@��� $��whois���� $��HTTP/0.9���� $��HTTP/1.0���� $��HTTP/1.1���� -	    ��Name$ SendRequestButton��$��Send Request��$��2��$��3��$��3��$��16(��� $��Timeout(ms)�� J� RenderTransformOrigin$ 0.294,0.652&�$��2��$��Center=�$��Right=�$��16(��� $��Hostname��$��1��$��Center=�$��Right=�$��16(��� $
��Name��$��3��$��Center=�$��Right=�$��16(��� $��Location��$ 0.398,0.571&�$��4��$��Center=�$��Right=�$��16(��� $
��Port��$��5��$��Center=�$��Right=�$��16(��� $��
Debug Mode��$��6��$��Center=�$��Right=�$��16(��� $��Server Response��$��7��$��Right=�$��Center=�$��16(��� -
    ��Name$ DebugCheckBox��$�� ��$��6��$��Stretch=�$��16(�$��Stretch=�$��102,16,16,12q�$ 0.5,0.5&� d� 	IsChecked �� *	DebugMode+$ window��$ TwoWay=�z�V� .~���  �� ScaleX$	 2.5Q� �� ScaleY$	 2.5Q��� �� T�  T� X$ 68Q��� $��1�� �� Orientation$ 
Horizontal=�$��85,16,16,10q�$��6��$��20��$��2��$	��198��.+�	� -    	�Name$ LookUpRadioButton��
 � Tag$ 0��$
 TrueY�$��Lookup��$��Center=�$��14(�$��0,0,5,0q�$��Center=�	� -   $ UpdateRadioButton��$ 1��$��Update��$��Center=�$��14(�$��Center=�