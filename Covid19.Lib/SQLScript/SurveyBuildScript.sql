--DROP TABLE Survey

--The Survey table is used to define specific surveys 

CREATE TABLE Survey
(
CompanyID Int NOT NULL,
SurveyID Nvarchar(40) NOT NULL,
SurveyName nvarchar(100) NOT NULL,
SurveyDesc nvarchar(255),
Active bit, --use this to deactivate the survey
NoteID uniqueidentifier NOT NULL,
CreatedByID uniqueidentifier NOT NULL,
CreatedByScreenID char(8) NOT NULL,
CreatedDateTime datetime NOT NULL,
LastModifiedByID uniqueidentifier NOT NULL,
LastModifiedByScreenID char(8) NOT NULL,
LastModifiedDateTime datetime NOT NULL,
CONSTRAINT [Survey_PK]
PRIMARY KEY CLUSTERED 
(
[CompanyID]
ASC,
[SurveyID]
ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

--DROP TABLE SurveyCollector
CREATE TABLE SurveyCollector
(
	CompanyID  Int not null,
	SurveyID   nvarchar(40) not null,
	CollectorID   Int not null,
    CollectorCD nvarchar(50) not null,
	ContactID Int not null,  
	CollectedDate DateTime not null,--the date the question was answered
	ExpirationDate datetime,
	CollectorStatus char(1), -- Status of record - New/Sent/responded/expired
	NoteID uniqueidentifier NOT NULL,
	CreatedByID uniqueidentifier NOT NULL,
	CreatedByScreenID char(8) NOT NULL,
	CreatedDateTime datetime NOT NULL,
	LastModifiedByID uniqueidentifier NOT NULL,
	LastModifiedByScreenID char(8) NOT NULL,
	LastModifiedDateTime datetime NOT NULL,
CONSTRAINT SurveyCollector_PK PRIMARY KEY CLUSTERED 
(
	CompanyID   ASC,
	SurveyID    ASC,
	CollectorID ASC
    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


--DROP TABLE SurveyRecipients
CREATE TABLE SurveyRecipients
(
	CompanyID Int NOT NULL,
	SurveyID nvarchar(40) NOT NULL,
	--EmployeeID Int NOT NULL,
	--UserID uniqueidentifier NOT NULL,
	LineNbr Int NOT NULL,
	ContactID Int NOT NULL,
	Active bit, --default to true, employee only participates if this is true. to opt an employee out uncheck this box.
    NoteID uniqueidentifier NOT NULL,
	CreatedByID uniqueidentifier NOT NULL,
	CreatedByScreenID char(8) NOT NULL,
	CreatedDateTime datetime NOT NULL,
	LastModifiedByID uniqueidentifier NOT NULL,
	LastModifiedByScreenID char(8) NOT NULL,
	LastModifiedDateTime datetime NOT NULL,
CONSTRAINT [SurveyRecipients_PK] PRIMARY KEY CLUSTERED 
(
	[CompanyID] ASC,
	[SurveyID]  ASC,
	--[EmployeeID] ASC
	--[UserID]    ASC
	[LineNbr] ASC
    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

--DROP TABLE SurveyQuestions
CREATE TABLE SurveyQuestions
(
	CompanyID  Int,
	SurveyID   nvarchar(40),
	QuestionID int,
	--This is the how the question should be worded to the user.
	Question nvarchar(500),
	--alpha numeric method to order the questions
	SortOrder nvarchar(20), 
	--Question Group will be used to group such things as 
	--Symptoms together in one block
	--we also have the option of defining this through another table.
	--Keeping it simple for now
	QuestionGroup nvarchar(40), 
	--I am working off of assumtions how the Attributes work.
	--Its assumed that we need to tie the quetion to a particular
	--Attribute as it will define the type and enumberations if 
	--applicable
	AttributeID nvarchar(10),
	NoteID uniqueidentifier NOT NULL,
	CreatedByID uniqueidentifier NOT NULL,
	CreatedByScreenID char(8) NOT NULL,
	CreatedDateTime datetime NOT NULL,
	LastModifiedByID uniqueidentifier NOT NULL,
	LastModifiedByScreenID char(8) NOT NULL,
	LastModifiedDateTime datetime NOT NULL,
CONSTRAINT SurveyQuestions_PK PRIMARY KEY CLUSTERED 
(
	CompanyID ASC,
	SurveyID  ASC,
	QuestionID ASC
    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]