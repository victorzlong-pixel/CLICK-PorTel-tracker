clear all
close all
clc

cd C:\Users\NODE_GS\Pictures\BackyardEOS\PLANETARY
dirName = uigetdir;
cd(dirName)
fnames = dir('*.jpg');

pic = imread(fnames(1).name);
gray_pic = rgb2gray(pic);

meanG = mean(gray_pic(:))
sd = std(double(gray_pic(:)))
noiseThreshold = meanG + 10*sd;

pic_thresh = gray_pic >= noiseThreshold;
figure
imshow(gray_pic)
figure
imshow(pic_thresh)

centroids = bwconncomp(pic_thresh);
num_pix = cellfun(@numel,centroids.PixelIdxList);
[biggest,idx] = max(num_pix);

