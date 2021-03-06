
/****** Oggetto:  Table [dbo].[SequenceData]    Data script: 03/07/2013 15:48:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Sequence_Data](
	[SeqName] [varchar](50) NOT NULL,
	[SeqValue] [bigint] NOT NULL,
	[DataInserimento] [datetime] NOT NULL,
	[DataAggiornamento] [datetime] NOT NULL,
 CONSTRAINT [PK_SequenceData] PRIMARY KEY CLUSTERED 
(
	[SeqName] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF


GO
/****** Oggetto:  StoredProcedure [dbo].[sp_SEQ_Drop]    Data script: 03/07/2013 15:48:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Simone Pelaia
-- Create date: 25/02/2013
-- Description:	Cancella in maniera atomica un contatore
-- =============================================
CREATE PROCEDURE [dbo].[sp_SEQ_Drop] 
	@seqName VARCHAR(50)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    BEGIN TRAN

	   DELETE FROM Sequence_Data WITH (SERIALIZABLE)
	   WHERE SeqName = @seqName
	  
	COMMIT TRAN
END


/****** Oggetto:  StoredProcedure [dbo].[sp_SEQ_GetNextValue]    Data script: 03/07/2013 15:49:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Simone Pelaia
-- Create date: 25/02/2013
-- Description:	Incrementa in maniera atomica un contatore
-- =============================================
CREATE PROCEDURE [dbo].[sp_SEQ_GetNextValue] 
	@seqName VARCHAR(50)
AS
BEGIN
	SET NOCOUNT ON;

    BEGIN TRAN
	   DECLARE @OUTTAB TABLE (SeqValue INT)
	   -- AGGIORNA SEQUENZA 
	   UPDATE Sequence_Data WITH (SERIALIZABLE)
	   SET SeqValue = SeqValue + 1, DataAggiornamento=CURRENT_TIMESTAMP
	   OUTPUT INSERTED.SeqValue INTO @OUTTAB -- RITORNA NUMERO INSERITO
	   WHERE SeqName = @seqName
	   --SEQUENZA NON TROVATA
	   IF @@ROWCOUNT = 0
	   BEGIN
		  --INSERIMENTO SEQUENZA
		  INSERT Sequence_Data (SeqName, SeqValue, DataInserimento, DataAggiornamento)
		  OUTPUT INSERTED.SeqValue INTO @OUTTAB -- RITORNA NUMEO INSERITO
		  VALUES (@seqName, 1, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP)
	   END

	   SELECT SeqValue FROM @OUTTAB

	COMMIT TRAN
END



/****** Oggetto:  StoredProcedure [dbo].[sp_SEQ_Reset]    Data script: 03/07/2013 15:49:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Simone Pelaia
-- Create date: 25/02/2013
-- Description:	Azzera in maniera atomica un contatore
-- =============================================
CREATE PROCEDURE [dbo].[sp_SEQ_Reset] 
	@seqName VARCHAR(50)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    BEGIN TRAN
	   UPDATE Sequence_Data WITH (SERIALIZABLE)
	   SET SeqValue = 0, DataAggiornamento = CURRENT_TIMESTAMP
	   WHERE SeqName = @seqName
	   IF @@ROWCOUNT = 0
	   BEGIN
		  INSERT Sequence_Data (SeqName, SeqValue, DataInserimento, DataAggiornamento)
		  VALUES (@seqName, 0, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP)
	   END

	COMMIT TRAN
END


