<?php
	$text1 = $_POST["pathFile"];
	$text2 = $_POST["text"];
	
	$cur_dir = getcwd(); 
	$saveFile = $cur_dir . '/Saves/' . $text1;
	
	if ($text1 != ""){
		$fp = fopen($saveFile, 'w');
		fwrite($fp, $text2);
		fclose($fp);
		print_r($text2);
	}
?>