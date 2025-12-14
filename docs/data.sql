INSERT INTO "Users"
(
	"Id",
	"Email",
	"PasswordHash",
	"FirstName",
	"LastName",
	"PhoneNumber",
	"DateOfBirth",
	"CreatedAt",
	"UpdatedAt",
    "IsDeleted",
    "DeletedAt"
) VALUES 
  (
	'a0eebc99-9c0b-4ef8-bb6d-6bb9bd380a11',
	'sarah.jonhson@example.com',
	'$2a$12$U.wcxRlL6o.YZvRRxVUyluBS/k9dJFKEI/71SIGwjg8DgKRIGaghW',
	'Sarah',
	'Jonhson',
	'+380-68-04-29896',
	'1985-05-12 00:00:00',
    '2023-01-15 08:30:00',
    '2023-01-15 08:30:00',
	false,
	NULL
  ),
  (
	'b1ffcd88-8d1a-3fe7-aa5c-5cc8ac271b22',
	'james.howlett@example.com',
	'$2a$12$iav/OMbrVpDUATfdUuR.TeG/EB65AsuDcNUAPqR30lWcQEKY6yi6W',
	'James',
	'Howlett',
	'+1-255-242-29866',
	'1986-10-10 00:00:00',
    '2023-02-20 14:15:00',
    '2023-03-01 09:00:00',
	false,
	NULL
  ),
  (
    'c2eedf77-7e2b-2ad6-bb4b-4bb7ab162c33',
    'diana.prince@example.com',
    '$2b$12$D4G5f18o7aMM.mWz9.8H/eN0s1/p.Q9r8.S7t6.U5v4.W3x2.Y1z0.A1b2',
    'Diana',
    'Prince',
    '+1-555-014-7744',
    '1990-03-22 00:00:00',
    '2023-03-05 11:45:00',
    '2023-06-10 16:20:00',
    false,
    NULL
),
(
    'd3aaef66-6f3c-1bd5-cc3a-3aa6ba051d44',
    'wade.wilson@example.com',
    '$2b$12$EXMPL.Hash.Gen.99.88.77.66.55.44.33.22.11.00.AA.BB.CC.DD.EE',
    'Wade',
    'Wilson',
    '+1-555-012-6655',
    '1992-11-04 00:00:00',
    '2023-04-01 09:00:00',
    '2023-11-01 10:00:00',
    true,
    '2023-11-01 10:00:00'
),
(
    'e4bbfa55-5e4d-0ae4-dd2f-2ff5cb940e55',
    'tony.stark@example.com',
    '$2b$12$LQ.v2.K9j.H8g.F7d.S6a.P5o.I4u.Y3t.R2e.W1q.M0n.B9v.C8x.Z7l.X',
    'Tony',
    'Stark',
    '+1-555-018-3300',
    '1970-05-29 00:00:00',
    '2022-12-01 10:00:00',
    '2022-12-01 10:00:00',
    false,
    NULL
);


   
  	
