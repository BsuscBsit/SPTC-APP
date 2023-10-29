-- phpMyAdmin SQL Dump
-- version 5.2.0
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1
-- Generation Time: Oct 29, 2023 at 02:44 AM
-- Server version: 10.4.27-MariaDB
-- PHP Version: 8.2.0

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `dtb_sptc`
--

-- --------------------------------------------------------

--
-- Table structure for table `tbl_address`
--

CREATE TABLE `tbl_address` (
  `id` int(11) NOT NULL,
  `address_line1` varchar(50) NOT NULL DEFAULT '',
  `address_line2` varchar(50) NOT NULL DEFAULT '',
  `house_no` varchar(20) DEFAULT '',
  `street_name` varchar(20) DEFAULT '',
  `barangay_subdivision` varchar(20) DEFAULT '',
  `city_municipality` varchar(20) DEFAULT '',
  `postal_code` varchar(6) DEFAULT '',
  `province` varchar(20) DEFAULT '',
  `country` varchar(20) DEFAULT '',
  `isDeleted` tinyint(1) NOT NULL DEFAULT 0
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Table structure for table `tbl_driver`
--

CREATE TABLE `tbl_driver` (
  `id` int(11) NOT NULL,
  `name_id` int(11) DEFAULT -1,
  `address_id` int(11) NOT NULL DEFAULT -1,
  `image_id` int(11) NOT NULL DEFAULT -1,
  `sign_id` int(11) DEFAULT -1,
  `remarks` varchar(100) DEFAULT NULL,
  `date_of_birth` date DEFAULT NULL,
  `contact_no` varchar(11) DEFAULT NULL,
  `emergency_person` varchar(50) DEFAULT NULL,
  `emergency_number` varchar(11) DEFAULT NULL,
  `date_of_mem` date DEFAULT NULL,
  `isDeleted` tinyint(1) NOT NULL DEFAULT 0
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Table structure for table `tbl_employee`
--

CREATE TABLE `tbl_employee` (
  `id` int(11) NOT NULL,
  `name_id` int(11) NOT NULL DEFAULT -1,
  `address_id` int(11) NOT NULL DEFAULT -1,
  `sign_id` int(11) NOT NULL DEFAULT -1,
  `image_id` int(11) NOT NULL DEFAULT -1,
  `password` varchar(50) DEFAULT NULL,
  `position_id` int(11) NOT NULL DEFAULT -1,
  `start_date` date DEFAULT NULL,
  `end_date` date DEFAULT NULL,
  `date_of_birth` date DEFAULT NULL,
  `contact_no` varchar(11) DEFAULT NULL,
  `isDeleted` tinyint(1) NOT NULL DEFAULT 0
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `tbl_employee`
--

INSERT INTO `tbl_employee` (`id`, `name_id`, `address_id`, `sign_id`, `image_id`, `password`, `position_id`, `start_date`, `end_date`, `date_of_birth`, `contact_no`, `isDeleted`) VALUES
(1, 1, -1, -1, -1, '751cb3f4aa17c36186f4856c8982bf27', 1, NULL, NULL, NULL, NULL, 0),
(2, -1, -1, -1, -1, '751cb3f4aa17c36186f4856c8982bf27', 2, NULL, NULL, NULL, NULL, 0),
(3, -1, -1, -1, -1, '751cb3f4aa17c36186f4856c8982bf27', 3, NULL, NULL, NULL, NULL, 0),
(4, -1, -1, -1, -1, '751cb3f4aa17c36186f4856c8982bf27', 4, NULL, NULL, NULL, NULL, 0),
(5, -1, -1, -1, -1, NULL, 5, NULL, NULL, NULL, NULL, 0);

-- --------------------------------------------------------

--
-- Table structure for table `tbl_franchise`
--

CREATE TABLE `tbl_franchise` (
  `id` int(11) NOT NULL,
  `body_number` int(11) NOT NULL DEFAULT -1,
  `operator_id` int(11) NOT NULL DEFAULT -1,
  `driver_id` int(11) NOT NULL DEFAULT -1,
  `owner_id` int(11) NOT NULL DEFAULT -1,
  `buying_date` date DEFAULT NULL,
  `last_franchise_id` int(11) NOT NULL DEFAULT -1,
  `mtop_no` varchar(20) DEFAULT NULL,
  `license_no` varchar(20) DEFAULT NULL,
  `voters_id_number` varchar(50) DEFAULT NULL,
  `tin_number` varchar(50) DEFAULT NULL,
  `isDeleted` tinyint(1) NOT NULL DEFAULT 0
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Table structure for table `tbl_id_history`
--

CREATE TABLE `tbl_id_history` (
  `id` int(11) NOT NULL,
  `date` date DEFAULT NULL,
  `owner_id` int(11) NOT NULL DEFAULT -1,
  `entity_type` varchar(10) NOT NULL DEFAULT 'OPERATOR',
  `franchise_id` int(11) NOT NULL DEFAULT -1,
  `is_printed` tinyint(1) NOT NULL DEFAULT 0,
  `isDeleted` tinyint(1) NOT NULL DEFAULT 0
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Table structure for table `tbl_image`
--

CREATE TABLE `tbl_image` (
  `id` int(11) NOT NULL,
  `image_source_bin` mediumblob DEFAULT NULL,
  `image_name` varchar(20) DEFAULT '',
  `isDeleted` tinyint(1) NOT NULL DEFAULT 0
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Table structure for table `tbl_loan_ledger`
--

CREATE TABLE `tbl_loan_ledger` (
  `id` int(11) NOT NULL,
  `franchise_id` int(11) NOT NULL DEFAULT -1,
  `date` date DEFAULT NULL,
  `amount` double NOT NULL DEFAULT 0,
  `details` varchar(100) DEFAULT NULL,
  `monthly_interest` double NOT NULL DEFAULT 0,
  `payment_dues` double NOT NULL DEFAULT 0,
  `terms_of_payment_month` int(11) NOT NULL DEFAULT 1,
  `isFullyPaid` tinyint(1) NOT NULL DEFAULT 0,
  `isDeleted` tinyint(1) NOT NULL DEFAULT 0
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Table structure for table `tbl_long_term_loan_ledger`
--

CREATE TABLE `tbl_long_term_loan_ledger` (
  `id` int(11) NOT NULL,
  `franchise_id` int(11) NOT NULL DEFAULT -1,
  `date` date DEFAULT NULL,
  `terms_of_payment_month` int(11) NOT NULL DEFAULT 1,
  `start_date` date DEFAULT NULL,
  `end_date` date DEFAULT NULL,
  `amount_loaned` double NOT NULL DEFAULT 0,
  `details` varchar(100) DEFAULT NULL,
  `processing_fee` double NOT NULL DEFAULT 0,
  `capital_buildup` double NOT NULL DEFAULT 0,
  `payment_dues` double NOT NULL DEFAULT 0,
  `isFullyPaid` tinyint(1) NOT NULL DEFAULT 0,
  `isDeleted` tinyint(1) NOT NULL DEFAULT 0
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Table structure for table `tbl_name`
--

CREATE TABLE `tbl_name` (
  `id` int(11) NOT NULL,
  `sex` tinyint(1) NOT NULL DEFAULT 0,
  `first_name` varchar(20) NOT NULL DEFAULT '',
  `middle_name` varchar(20) NOT NULL DEFAULT '',
  `last_name` varchar(20) NOT NULL DEFAULT '',
  `suffix` varchar(10) NOT NULL DEFAULT '',
  `isDeleted` tinyint(1) NOT NULL DEFAULT 0
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Table structure for table `tbl_operator`
--

CREATE TABLE `tbl_operator` (
  `id` int(11) NOT NULL,
  `name_id` int(11) DEFAULT -1,
  `address_id` int(11) NOT NULL DEFAULT -1,
  `image_id` int(11) NOT NULL DEFAULT -1,
  `sign_id` int(11) DEFAULT -1,
  `remarks` varchar(100) DEFAULT NULL,
  `date_of_birth` date DEFAULT NULL,
  `contact_no` varchar(11) DEFAULT NULL,
  `emergency_person` varchar(50) DEFAULT NULL,
  `emergency_number` varchar(11) DEFAULT NULL,
  `isOwner` tinyint(1) NOT NULL DEFAULT 0,
  `date_of_mem` date DEFAULT NULL,
  `tin_number` varchar(50) DEFAULT NULL,
  `voters_id_number` varchar(50) DEFAULT NULL,
  `isDeleted` tinyint(1) NOT NULL DEFAULT 0
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Table structure for table `tbl_payment_details`
--

CREATE TABLE `tbl_payment_details` (
  `id` int(11) NOT NULL,
  `ledger_id` int(11) NOT NULL DEFAULT -1,
  `isDownPayment` tinyint(1) NOT NULL DEFAULT 0,
  `isDivPat` tinyint(1) NOT NULL DEFAULT 0,
  `ledger_type` varchar(30) DEFAULT NULL,
  `date` date DEFAULT NULL,
  `reference_no` int(11) NOT NULL DEFAULT -1,
  `deposit` double NOT NULL DEFAULT 0,
  `penalties` double NOT NULL DEFAULT 0,
  `remarks` varchar(100) DEFAULT NULL,
  `balance` double NOT NULL DEFAULT 0,
  `isDeleted` tinyint(1) NOT NULL DEFAULT 0
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Table structure for table `tbl_position`
--

CREATE TABLE `tbl_position` (
  `id` int(11) NOT NULL,
  `title` varchar(50) DEFAULT NULL,
  `can_cnd` tinyint(1) NOT NULL DEFAULT 0,
  `can_cno` tinyint(1) NOT NULL DEFAULT 0,
  `can_cnf` tinyint(1) NOT NULL DEFAULT 0,
  `can_udp` tinyint(1) NOT NULL DEFAULT 0,
  `can_uop` tinyint(1) NOT NULL DEFAULT 0,
  `can_ufp` tinyint(1) NOT NULL DEFAULT 0,
  `can_dd` tinyint(1) NOT NULL DEFAULT 0,
  `can_do` tinyint(1) NOT NULL DEFAULT 0,
  `can_df` tinyint(1) NOT NULL DEFAULT 0,
  `can_ig` tinyint(1) NOT NULL DEFAULT 0,
  `can_cd` tinyint(1) NOT NULL DEFAULT 0,
  `can_co` tinyint(1) NOT NULL DEFAULT 0,
  `can_adv` tinyint(1) NOT NULL DEFAULT 0,
  `can_isl` tinyint(1) NOT NULL DEFAULT 0,
  `can_ill` tinyint(1) NOT NULL DEFAULT 0,
  `can_itl` tinyint(1) NOT NULL DEFAULT 0,
  `can_ud` tinyint(1) NOT NULL DEFAULT 0,
  `can_ubm` tinyint(1) NOT NULL DEFAULT 0,
  `can_rp` tinyint(1) NOT NULL DEFAULT 0,
  `can_pi` tinyint(1) NOT NULL DEFAULT 0,
  `isDeleted` tinyint(1) NOT NULL DEFAULT 0
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `tbl_position`
--

INSERT INTO `tbl_position` (`id`, `title`, `can_cnd`, `can_cno`, `can_cnf`, `can_udp`, `can_uop`, `can_ufp`, `can_dd`, `can_do`, `can_df`, `can_ig`, `can_cd`, `can_co`, `can_adv`, `can_isl`, `can_ill`, `can_itl`, `can_ud`, `can_ubm`, `can_rp`, `can_pi`, `isDeleted`) VALUES
(1, 'General Manager', 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0),
(2, 'Secretary', 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0),
(3, 'Treasurer', 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0),
(4, 'Bookeeper', 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0),
(5, 'Chairman', 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);

-- --------------------------------------------------------

--
-- Table structure for table `tbl_share_capital_ledger`
--

CREATE TABLE `tbl_share_capital_ledger` (
  `id` int(11) NOT NULL,
  `franchise_id` int(11) NOT NULL DEFAULT -1,
  `date` date DEFAULT NULL,
  `beginning_balance` double NOT NULL DEFAULT 0,
  `last_balance` double NOT NULL DEFAULT 0,
  `isDeleted` tinyint(1) NOT NULL DEFAULT 0
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Table structure for table `tbl_violation`
--

CREATE TABLE `tbl_violation` (
  `id` int(11) NOT NULL,
  `franchise_id` int(11) NOT NULL DEFAULT -1,
  `violation_level_count` int(11) NOT NULL DEFAULT 0,
  `violation_type_id` int(11) NOT NULL DEFAULT -1,
  `date` date DEFAULT NULL,
  `suspension_start` date DEFAULT NULL,
  `suspension_end` date DEFAULT NULL,
  `remarks` varchar(100) DEFAULT NULL,
  `name_id` int(11) NOT NULL DEFAULT -1,
  `isDeleted` tinyint(1) NOT NULL DEFAULT 0
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Table structure for table `tbl_violation_type`
--

CREATE TABLE `tbl_violation_type` (
  `id` int(11) NOT NULL,
  `title` varchar(50) NOT NULL DEFAULT '',
  `details` varchar(100) DEFAULT NULL,
  `num_of_days` int(11) NOT NULL DEFAULT 0,
  `entity_type` varchar(20) NOT NULL DEFAULT 'DRIVER',
  `isDeleted` tinyint(1) NOT NULL DEFAULT 0
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Indexes for dumped tables
--

--
-- Indexes for table `tbl_address`
--
ALTER TABLE `tbl_address`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `address_line1` (`address_line1`,`address_line2`);

--
-- Indexes for table `tbl_driver`
--
ALTER TABLE `tbl_driver`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `name_id` (`name_id`,`address_id`);

--
-- Indexes for table `tbl_employee`
--
ALTER TABLE `tbl_employee`
  ADD PRIMARY KEY (`id`);

--
-- Indexes for table `tbl_franchise`
--
ALTER TABLE `tbl_franchise`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `body_number` (`body_number`,`last_franchise_id`);

--
-- Indexes for table `tbl_id_history`
--
ALTER TABLE `tbl_id_history`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `owner_id` (`owner_id`,`entity_type`,`franchise_id`);

--
-- Indexes for table `tbl_image`
--
ALTER TABLE `tbl_image`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `image_name` (`image_name`);

--
-- Indexes for table `tbl_loan_ledger`
--
ALTER TABLE `tbl_loan_ledger`
  ADD PRIMARY KEY (`id`);

--
-- Indexes for table `tbl_long_term_loan_ledger`
--
ALTER TABLE `tbl_long_term_loan_ledger`
  ADD PRIMARY KEY (`id`);

--
-- Indexes for table `tbl_name`
--
ALTER TABLE `tbl_name`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `prefix` (`sex`,`first_name`,`middle_name`,`last_name`,`suffix`);

--
-- Indexes for table `tbl_operator`
--
ALTER TABLE `tbl_operator`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `name_id` (`name_id`,`address_id`);

--
-- Indexes for table `tbl_payment_details`
--
ALTER TABLE `tbl_payment_details`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `ref` (`reference_no`);

--
-- Indexes for table `tbl_position`
--
ALTER TABLE `tbl_position`
  ADD PRIMARY KEY (`id`);

--
-- Indexes for table `tbl_share_capital_ledger`
--
ALTER TABLE `tbl_share_capital_ledger`
  ADD PRIMARY KEY (`id`);

--
-- Indexes for table `tbl_violation`
--
ALTER TABLE `tbl_violation`
  ADD PRIMARY KEY (`id`);

--
-- Indexes for table `tbl_violation_type`
--
ALTER TABLE `tbl_violation_type`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `title` (`title`,`num_of_days`,`entity_type`) USING BTREE;

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `tbl_address`
--
ALTER TABLE `tbl_address`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `tbl_driver`
--
ALTER TABLE `tbl_driver`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `tbl_employee`
--
ALTER TABLE `tbl_employee`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;

--
-- AUTO_INCREMENT for table `tbl_franchise`
--
ALTER TABLE `tbl_franchise`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `tbl_id_history`
--
ALTER TABLE `tbl_id_history`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `tbl_image`
--
ALTER TABLE `tbl_image`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `tbl_loan_ledger`
--
ALTER TABLE `tbl_loan_ledger`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `tbl_long_term_loan_ledger`
--
ALTER TABLE `tbl_long_term_loan_ledger`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `tbl_name`
--
ALTER TABLE `tbl_name`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `tbl_operator`
--
ALTER TABLE `tbl_operator`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `tbl_payment_details`
--
ALTER TABLE `tbl_payment_details`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `tbl_position`
--
ALTER TABLE `tbl_position`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;

--
-- AUTO_INCREMENT for table `tbl_share_capital_ledger`
--
ALTER TABLE `tbl_share_capital_ledger`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `tbl_violation`
--
ALTER TABLE `tbl_violation`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `tbl_violation_type`
--
ALTER TABLE `tbl_violation_type`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
