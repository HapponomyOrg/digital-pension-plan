-- Set the client encoding to UTF-8
SET client_encoding = 'UTF8';

-- Set standard conforming strings to 'on'
SET standard_conforming_strings = 'on';

-- Set the search path
SELECT pg_catalog.set_config('search_path', '', false);

-- Create the postgres database
CREATE DATABASE postgres 
WITH TEMPLATE = template0 
ENCODING = 'UTF8' 
LOCALE_PROVIDER = libc 
LOCALE = 'en_US.utf8';

-- Comment on the database
COMMENT ON DATABASE postgres IS 'default administrative connection database';

-- Create auctions table
CREATE TABLE public.auctions (
                                 id integer NOT NULL,
                                 auction_id character varying(255) NOT NULL,
                                 session_id bigint,
                                 seller_id bigint,
                                 cards integer[] NOT NULL,
                                 amount integer NOT NULL,
                                 canceled boolean DEFAULT false,
                                 listed_at timestamp without time zone NOT NULL,
                                 canceled_at timestamp without time zone
);

-- Create auctions_id_seq sequence
CREATE SEQUENCE public.auctions_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;

-- Alter sequence for auctions table
ALTER SEQUENCE public.auctions_id_seq OWNED BY public.auctions.id;

-- Set default for id column in auctions table
ALTER TABLE ONLY public.auctions
ALTER COLUMN id SET DEFAULT nextval('public.auctions_id_seq'::regclass);

-- Create bids table
CREATE TABLE public.bids (
                             id integer NOT NULL,
                             auction_id bigint NOT NULL,
                             amount bigint NOT NULL,
                             buyer_id bigint,
                             bid_at timestamp without time zone NOT NULL,
                             canceled boolean DEFAULT false,
                             canceled_at timestamp without time zone,
                             rejected boolean DEFAULT false,
                             accepted boolean DEFAULT false,
                             settled_at timestamp without time zone,
                             counter_id integer
);

-- Create bids_id_seq sequence
CREATE SEQUENCE public.bids_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;

-- Alter sequence for bids table
ALTER SEQUENCE public.bids_id_seq OWNED BY public.bids.id;

-- Set default for id column in bids table
ALTER TABLE ONLY public.bids
ALTER COLUMN id SET DEFAULT nextval('public.bids_id_seq'::regclass);

-- Create card_hand_ins table
CREATE TABLE public.card_hand_ins (
                                      id integer NOT NULL,
                                      player_id bigint,
                                      card bigint NOT NULL,
                                      points bigint NOT NULL,
                                      handed_in_at timestamp without time zone NOT NULL
);

-- Create card_hand_ins_id_seq sequence
CREATE SEQUENCE public.card_hand_ins_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;

-- Alter sequence for card_hand_ins table
ALTER SEQUENCE public.card_hand_ins_id_seq OWNED BY public.card_hand_ins.id;

-- Set default for id column in card_hand_ins table
ALTER TABLE ONLY public.card_hand_ins
ALTER COLUMN id SET DEFAULT nextval('public.card_hand_ins_id_seq'::regclass);

-- Create donate_money table
CREATE TABLE public.donate_money (
                                     id integer NOT NULL,
                                     player_id bigint,
                                     amount bigint NOT NULL,
                                     donated_at timestamp without time zone NOT NULL
);

-- Create donate_money_id_seq sequence
CREATE SEQUENCE public.donate_money_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;

-- Alter sequence for donate_money table
ALTER SEQUENCE public.donate_money_id_seq OWNED BY public.donate_money.id;

-- Set default for id column in donate_money table
ALTER TABLE ONLY public.donate_money
ALTER COLUMN id SET DEFAULT nextval('public.donate_money_id_seq'::regclass);

-- Create donate_points table
CREATE TABLE public.donate_points (
                                      id integer NOT NULL,
                                      from_player_id bigint,
                                      to_player_id bigint,
                                      amount bigint NOT NULL,
                                      donated_at timestamp without time zone NOT NULL
);

-- Create donate_points_id_seq sequence
CREATE SEQUENCE public.donate_points_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;

-- Alter sequence for donate_points table
ALTER SEQUENCE public.donate_points_id_seq OWNED BY public.donate_points.id;

-- Set default for id column in donate_points table
ALTER TABLE ONLY public.donate_points
ALTER COLUMN id SET DEFAULT nextval('public.donate_points_id_seq'::regclass);

-- Create players table
CREATE TABLE public.players (
                                id integer NOT NULL,
                                session_id bigint NOT NULL,
                                session_player_id bigint NOT NULL,
                                name character varying(255),
                                age integer,
                                gender character varying(255)
);

-- Create players_id_seq sequence
CREATE SEQUENCE public.players_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;

-- Alter sequence for players table
ALTER SEQUENCE public.players_id_seq OWNED BY public.players.id;

-- Set default for id column in players table
ALTER TABLE ONLY public.players
ALTER COLUMN id SET DEFAULT nextval('public.players_id_seq'::regclass);

-- Create sessions table
CREATE TABLE public.sessions (
                                 id bigint NOT NULL,
                                 token bigint NOT NULL,
                                 start timestamp without time zone,
                                 stop timestamp without time zone,
                                 round_one boolean,
                                 round_two boolean,
                                 round_three boolean,
                                 round_four boolean,
                                 created_at timestamp without time zone NOT NULL
);

-- Alter table to add IDENTITY to id column for sessions table
ALTER TABLE public.sessions
ALTER COLUMN id ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public.sessions_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);

-- Create transactions table
CREATE TABLE public.transactions (
                                     id integer NOT NULL,
                                     auction_id bigint NOT NULL,
                                     bidding_id bigint,
                                     amount bigint NOT NULL,
                                     buyer_id bigint,
                                     settled_at timestamp without time zone NOT NULL
);

-- Create transactions_id_seq sequence
CREATE SEQUENCE public.transactions_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;

-- Alter sequence for transactions table
ALTER SEQUENCE public.transactions_id_seq OWNED BY public.transactions.id;

-- Set default for id column in transactions table
ALTER TABLE ONLY public.transactions
ALTER COLUMN id SET DEFAULT nextval('public.transactions_id_seq'::regclass);

-- Add primary key constraints
ALTER TABLE ONLY public.auctions
    ADD CONSTRAINT auctions_pkey PRIMARY KEY (auction_id, id);

ALTER TABLE ONLY public.bids
    ADD CONSTRAINT bids_pkey PRIMARY KEY (id, auction_id);

ALTER TABLE ONLY public.card_hand_ins
    ADD CONSTRAINT card_hand_ins_pkey PRIMARY KEY (id);

ALTER TABLE ONLY public.donate_money
    ADD CONSTRAINT donate_money_pkey PRIMARY KEY (id);

ALTER TABLE ONLY public.donate_points
    ADD CONSTRAINT donate_points_pkey PRIMARY KEY (id);

ALTER TABLE ONLY public.players
    ADD CONSTRAINT pk_players PRIMARY KEY (session_player_id, session_id);

ALTER TABLE ONLY public.sessions
    ADD CONSTRAINT sessions_pkey PRIMARY KEY (id);

ALTER TABLE ONLY public.transactions
    ADD CONSTRAINT transactions_pkey PRIMARY KEY (id, auction_id);

-- Add unique constraints
ALTER TABLE ONLY public.auctions
    ADD CONSTRAINT auctions_pk UNIQUE (id);

ALTER TABLE ONLY public.bids
    ADD CONSTRAINT bids_pk UNIQUE (id);

ALTER TABLE ONLY public.players
    ADD CONSTRAINT players_pk UNIQUE (id);

-- Add foreign key constraints
ALTER TABLE ONLY public.bids
    ADD CONSTRAINT fk_auction_id FOREIGN KEY (auction_id) REFERENCES public.auctions(id);

ALTER TABLE ONLY public.transactions
    ADD CONSTRAINT fk_bidding_id FOREIGN KEY (bidding_id) REFERENCES public.bids(id);

ALTER TABLE ONLY public.bids
    ADD CONSTRAINT fk_buyer FOREIGN KEY (buyer_id) REFERENCES public.players(id);

ALTER TABLE ONLY public.transactions
    ADD CONSTRAINT fk_buyer FOREIGN KEY (buyer_id) REFERENCES public.players(id);

ALTER TABLE ONLY public.donate_points
    ADD CONSTRAINT fk_from FOREIGN KEY (from_player_id) REFERENCES public.players(id);

ALTER TABLE ONLY public.donate_money
    ADD CONSTRAINT fk_player_id FOREIGN KEY (player_id) REFERENCES public.players(id);

ALTER TABLE ONLY public.auctions
    ADD CONSTRAINT fk_seller_id FOREIGN KEY (seller_id) REFERENCES public.players(id);

ALTER TABLE ONLY public.auctions
    ADD CONSTRAINT fk_session FOREIGN KEY (session_id) REFERENCES public.sessions(id);

ALTER TABLE ONLY public.donate_points
    ADD CONSTRAINT fk_to FOREIGN KEY (to_player_id) REFERENCES public.players(id);

ALTER TABLE ONLY public.card_hand_ins
    ADD CONSTRAINT fk_to FOREIGN KEY (player_id) REFERENCES public.players(id);
