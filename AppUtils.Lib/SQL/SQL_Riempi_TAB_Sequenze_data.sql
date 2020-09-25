SELECT * FROM sequence_data;

-- DELETE FROM sequence_data;

-- mysql

-- SELECT COALESCE(RIGHT(MAX(NumeroProtocollo) , 4) + 1, 1) AS Prog FROM protocolli WHERE DataProtocollo="2008-02-04";
/*	
INSERT INTO sequence_data (SeqName,SeqValue,DataInserimento,DataAggiornamento)
	SELECT 
		DATE_FORMAT(p.dataprotocollo,'PROTO_%Y_%m_%d') ,
		COALESCE(RIGHT(MAX(p.NumeroProtocollo) , 4) + 100, 1),
		CURRENT_TIMESTAMP(),
		CURRENT_TIMESTAMP()
	FROM protocolli p
	GROUP BY p.dataprotocollo
;
*/
-- mssql

/*
INSERT INTO sequence_data (SeqName,SeqValue,DataInserimento,DataAggiornamento)
	SELECT 
		'PROTO_'+REPLACE(CONVERT(VARCHAR, p.dataprotocollo, 111), '/', '_') ,
		COALESCE(RIGHT(MAX(p.NumeroProtocollo) , 4) + 100, 1),
		CURRENT_TIMESTAMP,
		CURRENT_TIMESTAMP
	FROM protocolli p
	GROUP BY p.dataprotocollo
;
*/