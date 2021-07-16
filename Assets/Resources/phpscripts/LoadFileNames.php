<?php
	
	$cur_dir = getcwd();
	$filecount=0;
	$dir_name = $cur_dir . '/Saves/*.json';
	$files = glob($dir_name, GLOB_BRACE);
	$filecount = count($files);
	for ($i = 0; $i < $filecount; $i++) {
			$base= basename($files[$i])."?";
			print_r($base);
		}
		
?>



