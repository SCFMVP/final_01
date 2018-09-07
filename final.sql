/*
Navicat MySQL Data Transfer

Source Server         : Iot_rub
Source Server Version : 50721
Source Host           : localhost:3306
Source Database       : final

Target Server Type    : MYSQL
Target Server Version : 50721
File Encoding         : 65001

Date: 2018-08-05 22:55:05
*/

SET FOREIGN_KEY_CHECKS=0;
-- ----------------------------
-- Table structure for `dtfunction`
-- ----------------------------
DROP TABLE IF EXISTS `dtfunction`;
CREATE TABLE `dtfunction` (
  `fid` varchar(3) NOT NULL,
  `fname` text,
  `flag` text,
  `uflag` text,
  PRIMARY KEY (`fid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of dtfunction
-- ----------------------------
INSERT INTO `dtfunction` VALUES ('1', 'hello', 'x', 'xx');
INSERT INTO `dtfunction` VALUES ('2', 'qw', 'x', 'xxx');

-- ----------------------------
-- Table structure for `dtrole`
-- ----------------------------
DROP TABLE IF EXISTS `dtrole`;
CREATE TABLE `dtrole` (
  `rid` varchar(3) NOT NULL,
  `rname` text,
  `flag` text,
  PRIMARY KEY (`rid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of dtrole
-- ----------------------------
INSERT INTO `dtrole` VALUES ('1', 'hello', 'yy');
INSERT INTO `dtrole` VALUES ('2', 'qw', 'yyy');

-- ----------------------------
-- Table structure for `dtuser`
-- ----------------------------
DROP TABLE IF EXISTS `dtuser`;
CREATE TABLE `dtuser` (
  `uid` varchar(3) NOT NULL,
  `uname` text,
  `pwd` text,
  `uflag` text,
  PRIMARY KEY (`uid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of dtuser
-- ----------------------------
INSERT INTO `dtuser` VALUES ('1', 'zhao', 'asdac', 'asda');
INSERT INTO `dtuser` VALUES ('2', 'niu', '6C4A4C9281F362CBD6E548DD', '0000');
INSERT INTO `dtuser` VALUES ('3', 'curry', '6C4A4C9281F362CBD6E548DD', '0000');

-- ----------------------------
-- Table structure for `help`
-- ----------------------------
DROP TABLE IF EXISTS `help`;
CREATE TABLE `help` (
  `id` varchar(3) NOT NULL,
  `name` text,
  `flag` text,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of help
-- ----------------------------
INSERT INTO `help` VALUES ('1', '按钮', '三个');
INSERT INTO `help` VALUES ('2', '文本框', '不可为空');
INSERT INTO `help` VALUES ('3', '异常', '会退出');

-- ----------------------------
-- Table structure for `tb_device`
-- ----------------------------
DROP TABLE IF EXISTS `tb_device`;
CREATE TABLE `tb_device` (
  `deviceId` varchar(10) NOT NULL,
  `位置` text,
  `负责人` text,
  `部署日期` text,
  `报警器` text,
  `分贝检测` text,
  PRIMARY KEY (`deviceId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of tb_device
-- ----------------------------
INSERT INTO `tb_device` VALUES ('001', '12.1233,12.12331(西苑)', 'worker_01', '2018/7/27', '有', null);
INSERT INTO `tb_device` VALUES ('002', '12.2121,213,123123(图书馆)', 'worker_02', '2018/7/27', '无', null);

-- ----------------------------
-- Table structure for `tb_worker`
-- ----------------------------
DROP TABLE IF EXISTS `tb_worker`;
CREATE TABLE `tb_worker` (
  `workerId` varchar(12) NOT NULL,
  `姓名` text,
  `性别` text,
  `年龄` int(4) DEFAULT NULL,
  `负责区域` text,
  `工资` int(20) DEFAULT NULL,
  PRIMARY KEY (`workerId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of tb_worker
-- ----------------------------
INSERT INTO `tb_worker` VALUES ('001', 'worker_01', '男', '37', '北苑', '2000');
INSERT INTO `tb_worker` VALUES ('002', 'worker_02', '男', '40', '北苑', '2000');
INSERT INTO `tb_worker` VALUES ('003', 'worker_03', '男', '40', '图书馆', '2000');
INSERT INTO `tb_worker` VALUES ('004', 'worker_04', '男', '40', '西苑', '2000');
INSERT INTO `tb_worker` VALUES ('005', 'worker_05', '男', '40', '西苑', '2000');
INSERT INTO `tb_worker` VALUES ('006', 'worker_06', '男', '40', '西苑', '2000');
INSERT INTO `tb_worker` VALUES ('007', 'worker_07', '男', '40', '西苑', '2000');
INSERT INTO `tb_worker` VALUES ('008', 'worker_08', '男', '40', '西苑', '2000');
INSERT INTO `tb_worker` VALUES ('009', 'worker_09', '男', '40', '西苑', '2000');
INSERT INTO `tb_worker` VALUES ('010', 'worker_10', '女', '39', '图书馆', '2000');
INSERT INTO `tb_worker` VALUES ('011', 'worker_11', '女', '42', '图书馆', '2000');
