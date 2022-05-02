Imports Newtonsoft
Namespace Youtube
    Public Class YoutubeDL
        Public Property YoutubeDLLocation As String
        Private Property UniversalSplitter As Char() = New Char() {"\r", "\n", "\r\n"}
        Public Class YoutubeVideo
            Property AlternateTitle As String
            Property AudioCodec As String
            Property AudioOnly As New List(Of Video)
            Property Author As String
            Property AuthorID As String
            Property AuthorURL As String
            Property Categories As String()
            Property Chapters As List(Of Chapter)
            Property Creator As String
            Property Description As String
            Property DirectURL As String
            Property DislikeCount As Integer
            Property Duration As TimeSpan
            Property FileName As String
            Property FPS As Integer
            Property FullTitle As String
            Property HQAudio As Video
            Property HQMixed As Video
            Property HQThumbnail As Thumbnail
            Property HQVideo As Video
            Property JSONDump As String
            Property LikeCount As Integer
            Property LQAudio As Video
            Property LQMixed As Video
            Property LQThumbnail As Thumbnail
            Property LQVideo As Video
            Property MixedOnly As New List(Of Video)
            Property Playlist As YoutubePlaylist
            Property Properties As MediaProperties
            Property RequestQuality As Quality
            Property Subtitles As New List(Of List(Of Subtitle))
            Property Tags As New List(Of String)
            Property Thumbnails As New List(Of Thumbnail)
            Property ThumbnailURL As String
            Property Title As String
            Property UploadDate As Date
            Property URL As String
            Property VideoCodec As String
            Property VideoExtension As String
            Property VideoOnly As New List(Of Video)
            Property Videos As New List(Of Video)
            Property ViewCount As Integer
            Public Class Thumbnail
                Public Property ID As Integer
                Public Property URL As String
                Public Property Width As Integer
                Public Property Height As Integer
                Public Property Resolution As String
            End Class
            Public Class Subtitle
                Public Property Language As String
                Public Property RawData As String
                Public Property Extension As String
            End Class
            Public Class Video
                Public Property AudioBitrate As String
                Public Property AudioSampleRate As String
                Public Property AudioCodec As String
                Public Property Container As String
                Public Property DirectURL As String
                Public Property Extension As String
                Public Property FileSize As String
                Public Property Format As String
                Public Property FormatID As String
                Public Property FormatNote As String
                Public Property FPS As String
                Public Property Height As String
                Public Property HTTPHeaders As HTTPHeader
                Public Property Protocol As String
                Public Property Quality As String
                Public Property TotalBitrate As String
                Public Property Type As FileType
                Public Property VideoBitrate As String
                Public Property VideoCodec As String
                Public Property Width As String
                Public Enum FileType
                    Audio
                    Video
                    Mixed
                End Enum
                Public Class HTTPHeader
                    Public Property AcceptLanguage As String
                    Public Property AcceptCharset As String
                    Public Property UserAgent As String
                    Public Property AcceptEncoding As String
                    Public Property Accept As String
                End Class
            End Class
            Public Class YoutubePlaylist
                Property Index As Integer
                Property Playlist_ID As String
                Property Title As String
                Property Uploader As String
                Property Uploader_ID As String
            End Class
            Public Class MediaProperties
                Property Album As String
                Property AlbumArtist As String
                Property AlbumType As String
                Property Artists As String()
                Property DiscNumber As Integer
                Property Genres As String()
                Property ReleaseYear As Integer
                Property Track As String
                Property Track_ID As String
                Property TrackNumber As Integer
            End Class
            Public Class Chapter
                Property EndTime As TimeSpan
                Property StarTime As TimeSpan
                Property Title As String
            End Class
            Public Enum Quality
                best
                worst
                bestvideo
                worstvideo
                bestaudio
                worstaudio
            End Enum
            Public Shadows Function ToString() As String
                Return Title & Environment.NewLine & Author & Environment.NewLine & URL & Environment.NewLine & DirectURL & Environment.NewLine & RequestQuality.ToString
            End Function
        End Class
        Public Class Resolution
            Public Property Height As Integer
            Public Property Width As Integer
            Public Sub New(H As Integer, W As Integer)
                Height = H
                Width = W
            End Sub
        End Class
        ''' <summary>
        '''Initialize a new instance of <see cref="YoutubeDL"/>
        ''' </summary>        
        ''' <exception cref="ArgumentException"/>
        ''' <param name="YTDLPath">Youtube-DL path</param>
        Public Sub New(YTDLPath As String)
            If IO.File.Exists(YTDLPath) Then
                YoutubeDLLocation = YTDLPath
            Else
                Throw New ArgumentNullException("YoutubeDL doesn't exists in the given path.")
            End If
        End Sub
        Public Shared Function IsYoutubeLink(URL As String) As Boolean
            'https://www.youtube.com/watch?v=0xSiBpUdW4E&list=RDCLAK5uy_kvhjcPWzH7xZL-WnqGbiA_euQGy5_cbHI&index=19
            If URL.Contains("youtube.com/watch?v=") Then Return True
            'youtu.be
            If URL.ToLower.Contains("youtu.be") Then Return True 'https://youtu.be/PKfxmFU3lWY?list=RDPKfxmFU3lWY
            Return False
        End Function
         <System.Runtime.InteropServices.DllImport("wininet.dll")>
        Private Shared Function InternetGetConnectedState(ByRef Description As Integer, ByVal ReservedValue As Integer) As Boolean
        End Function
        Public Shared Function CheckInternetConnection() As Boolean
            Try
                Dim ConnDesc As Integer
                Return InternetGetConnectedState(ConnDesc, 0)
            Catch
                Return False
            End Try
        End Function
        Public Shared Function IsYoutubePlaylistLink(URL As String) As Boolean
            'https://www.youtube.com/playlist?list=PLzSjbEiFKZ_w9zWXjVSTLi5FUlcPHwQCc
            If URL.Contains("youtube.com/playlist?list=") Then Return True
            Return False
        End Function
        Public Async Function GetVideo(URL As String, Q As YoutubeVideo.Quality) As Task(Of YoutubeVideo)
            If CheckInternetConnection Then
                Return Await Task.Run(Function()
                                          Dim YoutubeDL As Process = Process.Start(New ProcessStartInfo(YoutubeDLLocation, "-s -e -g -f " & Q.ToString & Space(1) & URL) With {.RedirectStandardOutput = True, .UseShellExecute = False, .WindowStyle = ProcessWindowStyle.Hidden, .CreateNoWindow = True})
                                          Dim Info = YoutubeDL.StandardOutput.ReadToEnd.Split(New Char() {vbCr, vbCrLf, vbLf})
                                          Dim Vid As New YoutubeVideo With {.Title = Info(0), .URL = URL, .DirectURL = Info(1), .RequestQuality = Q}
                                          Return Vid
                                      End Function)
            Else
                Return Nothing
            End If
        End Function
        Public Async Function SearchVideo(Query As String, Limit As Integer) As Task(Of String())
            If Limit > 1 Then
                If CheckInternetConnection Then
                    Return Await Task.Run(Function()
                                              Dim YoutubeDL As Process = Process.Start(New ProcessStartInfo(YoutubeDLLocation, """ytsearch" & Limit & ":" & Query & """ --get-id") With {.RedirectStandardOutput = True, .UseShellExecute = False, .WindowStyle = ProcessWindowStyle.Hidden, .CreateNoWindow = True})
                                              Dim out = YoutubeDL.StandardOutput.ReadToEnd
                                              Return out.Split(New Char() {"\n", "\r", "\r\n"})
                                          End Function)
                End If
            End If
            Return Nothing
        End Function
        Public Async Function SearchVideoAndDump(Query As String, Limit As Integer) As Task(Of IEnumerable(Of YoutubeVideo))
            If Limit > 1 Then
                If CheckInternetConnection Then
                    Return Await Task.Run(Async Function()
                                              Try
                                                  Dim YoutubeDL As Process = Process.Start(New ProcessStartInfo(YoutubeDLLocation, """ytsearch" & Limit & ":" & Query & """ -J") With {.RedirectStandardOutput = True, .UseShellExecute = False, .WindowStyle = ProcessWindowStyle.Hidden, .CreateNoWindow = True})
                                                  Dim sout = (Await YoutubeDL.StandardOutput.ReadToEndAsync) '.Split(New Char() {"\n", "\r", "\r\n"})
                                                  Dim jsout = Json.Linq.JObject.Parse(sout)

                                                  Dim Videos As New List(Of YoutubeVideo)
                                                  For Each rawjson In jsout("entries")
                                                      Videos.Add(Await ParseYoutubeJson(rawjson.ToString))
                                                  Next
                                                  Return Videos
                                              Catch ex As Exception
                                                  Utils.WriteConsoleError(ex)
                                              End Try
                                          End Function)
                End If
            End If
            Return Nothing
        End Function
        Public Async Function SearchVideo(Query As String) As Task(Of String)
            If CheckInternetConnection Then
                Return Await Task.Run(Function()
                                          Dim YoutubeDL As Process = Process.Start(New ProcessStartInfo(YoutubeDLLocation, """ytsearch:" & Query & """ --get-id") With {.RedirectStandardOutput = True, .UseShellExecute = False, .WindowStyle = ProcessWindowStyle.Hidden, .CreateNoWindow = True})
                                          Return YoutubeDL.StandardOutput.ReadToEnd
                                      End Function)
            Else
                Return Nothing
            End If
        End Function
        Public Async Function SearchVideoAndDump(Query As String) As Task(Of YoutubeVideo)
            If CheckInternetConnection Then
                Return Await Task.Run(Async Function()
                                          Dim YoutubeDL As Process = Process.Start(New ProcessStartInfo(YoutubeDLLocation, """ytsearch:" & Query & """ -j") With {.RedirectStandardOutput = True, .UseShellExecute = False, .WindowStyle = ProcessWindowStyle.Hidden, .CreateNoWindow = True})
                                          Dim Info = YoutubeDL.StandardOutput.ReadToEnd
                                          Return Await ParseYoutubeJson(Info)
                                      End Function)
            Else
                Return Nothing
            End If
        End Function
        Public Async Function DumpAndManagePlaylist(URL As String, Optional SkipURLCheck As Boolean = False) As Task(Of IEnumerable(Of YoutubeVideo))
            If CheckInternetConnection Then
                If SkipURLCheck = False Then
                    If Not IsYoutubePlaylistLink(URL) Then Return Nothing
                End If

                Dim YoutubeDL As Process = Process.Start(New ProcessStartInfo(YoutubeDLLocation, "-j " & URL) With {.RedirectStandardOutput = True, .UseShellExecute = False, .WindowStyle = ProcessWindowStyle.Hidden, .CreateNoWindow = True})
                Dim Info = YoutubeDL.StandardOutput.ReadToEnd
            End If
        End Function
        Public Async Function ParseYoutubeJson(StrJson As String) As Task(Of YoutubeVideo)
            Return Await Task.Run(Function()
                                      Try
                                          Dim Info = StrJson
                                          If String.IsNullOrEmpty(Info.Trim) Then Throw New InvalidOperationException
                                          'Acquiring Info        
                                          Dim ParsedInfo = Json.Linq.JObject.Parse(Info)
                                          Dim FullTitle = ParsedInfo("fulltitle")
                                          Dim AltTitle = ParsedInfo("alt_title")
                                          Dim AudioCodec = ParsedInfo("acodec")
                                          Dim Author = ParsedInfo("channel")
                                          Dim AuhorID = ParsedInfo("channel_id")
                                          Dim AuthorURL = ParsedInfo("channel_url")
                                          Dim RawCategories = ParsedInfo("categories")
                                          Dim Categories = String.Join(Environment.NewLine, RawCategories).Split(UniversalSplitter)
                                          Dim Creator = ParsedInfo("creator")
                                          Dim Description = ParsedInfo("description")
                                          Dim RawDirectURLS = ParsedInfo("formats")
                                          'Dim RawDirectURLS = ParsedInfo("requested_formats")
                                          Dim DirectURLS As New List(Of YoutubeVideo.Video)
                                          Dim AudioOnlyURLS As New List(Of YoutubeVideo.Video)
                                          Dim VideoOnlyURLS As New List(Of YoutubeVideo.Video)
                                          Dim MixedOnlyURLS As New List(Of YoutubeVideo.Video)
                                          For Each RURL As Json.Linq.JToken In RawDirectURLS
                                              Dim CVideo As New YoutubeVideo.Video
                                              CVideo.AudioCodec = RURL("acodec")
                                              CVideo.Container = RURL("container")
                                              CVideo.DirectURL = RURL("url")
                                              CVideo.Extension = RURL("ext")
                                              CVideo.FileSize = RURL("filesize")
                                              CVideo.Format = RURL("format")
                                              CVideo.FormatID = RURL("format_id")
                                              CVideo.FormatNote = RURL("format_note")
                                              CVideo.FPS = RURL("fps")
                                              CVideo.Height = RURL("height")
                                              Dim RawHTTPHeader = RURL("http_headers")
                                              Dim HTTPHeader As New YoutubeVideo.Video.HTTPHeader
                                              HTTPHeader.Accept = RawHTTPHeader("Accept")
                                              HTTPHeader.AcceptCharset = RawHTTPHeader("Accept-Charset")
                                              HTTPHeader.AcceptEncoding = RawHTTPHeader("Accept-Encoding")
                                              HTTPHeader.AcceptLanguage = RawHTTPHeader("Accept-Language")
                                              HTTPHeader.UserAgent = RawHTTPHeader("User-Agent")
                                              CVideo.HTTPHeaders = HTTPHeader
                                              CVideo.Protocol = RURL("protocol")
                                              CVideo.Quality = RURL("quality")
                                              CVideo.VideoCodec = RURL("vcodec")
                                              CVideo.Width = RURL("width")
                                              If CVideo.AudioCodec <> "none" AndAlso CVideo.VideoCodec <> "none" Then
                                                  CVideo.Type = YoutubeVideo.Video.FileType.Mixed
                                              ElseIf CVideo.AudioCodec <> "none" AndAlso CVideo.VideoCodec = "none" Then
                                                  CVideo.Type = YoutubeVideo.Video.FileType.Audio
                                              ElseIf CVideo.AudioCodec = "none" AndAlso CVideo.VideoCodec <> "none" Then
                                                  CVideo.Type = YoutubeVideo.Video.FileType.Video
                                              End If
                                              Select Case CVideo.Type
                                                  Case YoutubeVideo.Video.FileType.Mixed
                                                      CVideo.VideoBitrate = RURL("vbr")
                                                      CVideo.AudioSampleRate = RURL("asr")
                                                      CVideo.TotalBitrate = RURL("TotalBitrate")
                                                      CVideo.AudioBitrate = RURL("abr")
                                                      MixedOnlyURLS.Add(CVideo)
                                                  Case YoutubeVideo.Video.FileType.Audio
                                                      CVideo.AudioSampleRate = RURL("asr")
                                                      CVideo.TotalBitrate = RURL("TotalBitrate")
                                                      CVideo.AudioBitrate = RURL("abr")
                                                      AudioOnlyURLS.Add(CVideo)
                                                  Case YoutubeVideo.Video.FileType.Video
                                                      CVideo.VideoBitrate = RURL("vbr")
                                                      CVideo.AudioSampleRate = RURL("asr")
                                                      CVideo.TotalBitrate = RURL("TotalBitrate")
                                                      VideoOnlyURLS.Add(CVideo)
                                              End Select
                                              DirectURLS.Add(CVideo)
                                          Next
                                          AudioOnlyURLS.OrderBy(Function(k) k.FileSize)
                                          VideoOnlyURLS.OrderBy(Function(k) k.Width)
                                          MixedOnlyURLS.OrderBy(Function(k) k.Width)
                                          Dim HQAudio = AudioOnlyURLS.Last
                                          Dim LQAudio = AudioOnlyURLS.First
                                          Dim HQVideo = VideoOnlyURLS.Last
                                          Dim LQVideo = VideoOnlyURLS.First
                                          Dim HQMixed = MixedOnlyURLS.Last
                                          Dim LQMixed = MixedOnlyURLS.First
                                          Dim DislikeCount = ParsedInfo("dislike_count")
                                          Dim Duration = TimeSpan.FromSeconds(ParsedInfo("duration"))
                                          Dim FileName = ParsedInfo("_filename")
                                          Dim FPS = ParsedInfo("fps")
                                          Dim LikeCount = ParsedInfo("like_count")
                                          Dim RawSubtitles = ParsedInfo("subtitles")
                                          Dim Subtitles As New List(Of List(Of YoutubeVideo.Subtitle))
                                          If RawSubtitles IsNot Nothing Then
                                              For Each Subtitle As Json.Linq.JToken In RawSubtitles
                                                  Dim CSubtitle As New List(Of YoutubeVideo.Subtitle)
                                                  For Each _Subtitle As Json.Linq.JToken In Subtitle
                                                      For Each __Subtitle As Json.Linq.JToken In _Subtitle
                                                          Dim CCSubtitle As New YoutubeVideo.Subtitle
                                                          CCSubtitle.Language = CType(Subtitle, Json.Linq.JProperty).Name
                                                          CCSubtitle.Extension = __Subtitle("ext") 'String.Join(Environment.NewLine, _Subtitle("ext")).Split(UniversalSplitter) '_Subtitle("ext")
                                                          CCSubtitle.RawData = __Subtitle("url")
                                                          CSubtitle.Add(CCSubtitle)
                                                      Next
                                                  Next
                                                  Subtitles.Add(CSubtitle)
                                              Next
                                          End If
                                          Dim RawTags = ParsedInfo("tags")
                                          Dim Tags As New List(Of String)
                                          For Each Tag In RawTags
                                              Tags.Add(CType(Tag, Json.Linq.JValue).Value)
                                          Next
                                          Dim RawThumbnails = ParsedInfo("thumbnails")
                                          Dim Thumbnails As New List(Of YoutubeVideo.Thumbnail)
                                          For Each Thumbnail As Json.Linq.JToken In RawThumbnails
                                              Dim CThumbnail As New YoutubeVideo.Thumbnail
                                              CThumbnail.ID = Thumbnail("id")
                                              CThumbnail.Resolution = Thumbnail("resolution")
                                              CThumbnail.URL = Thumbnail("url")
                                              CThumbnail.Height = Thumbnail("height")
                                              CThumbnail.Width = Thumbnail("width")
                                              Thumbnails.Add(CThumbnail)
                                          Next
                                          Thumbnails.OrderBy(Function(k) k.Width)
                                          Dim HQThumbnail = Thumbnails.Last
                                          Dim LQThumbnail = Thumbnails.First
                                          Dim ThumbnailURL = ParsedInfo("thumbnail")
                                          'Media Properties
                                          Dim Track = ParsedInfo("track")
                                          Dim Artist = ParsedInfo("artist")
                                          Dim Artists As String()
                                          Try
                                              Artists = Artist.ToString.Split(",")
                                          Catch ex As Exception
                                              Artists = {}
                                          End Try
                                          Dim TrackNumber = ParsedInfo("track_number")
                                          Dim TrackID = ParsedInfo("track_id")
                                          Dim Genre = ParsedInfo("genre")
                                          Dim Genres As String()
                                          Try
                                              Genres = Genre.ToString.Split(",")
                                          Catch ex As Exception
                                              Genre = {}
                                          End Try
                                          Dim Album = ParsedInfo("album")
                                          Dim AlbumType = ParsedInfo("album_type")
                                          Dim AlbumArtist = ParsedInfo("album_artist")
                                          Dim DiscNumber = ParsedInfo("disc_number")
                                          Dim ReleaseYear = ParsedInfo("release_year")
                                          Dim MediaInfo As New YoutubeVideo.MediaProperties
                                          Try
                                              MediaInfo.Album = Album
                                          Catch
                                          End Try
                                          Try
                                              MediaInfo.AlbumArtist = AlbumArtist
                                          Catch
                                          End Try
                                          Try
                                              MediaInfo.AlbumType = AlbumType
                                          Catch
                                          End Try
                                          Try
                                              MediaInfo.Artists = Artists
                                          Catch
                                          End Try
                                          Try
                                              MediaInfo.DiscNumber = DiscNumber
                                          Catch
                                          End Try
                                          Try
                                              MediaInfo.Genres = Genres
                                          Catch
                                          End Try
                                          Try
                                              MediaInfo.ReleaseYear = ReleaseYear
                                          Catch
                                          End Try
                                          Try
                                              MediaInfo.Track = Track
                                          Catch
                                          End Try
                                          Try
                                              MediaInfo.TrackNumber = TrackNumber
                                          Catch
                                          End Try
                                          Try
                                              MediaInfo.Track_ID = TrackID
                                          Catch
                                          End Try
                                          'Playlist Properties
                                          Dim Playlist = ParsedInfo("playlist")
                                          Dim PlaylistIndex = ParsedInfo("playlist_index")
                                          Dim PlaylistID = ParsedInfo("playlist_id")
                                          Dim PlaylistTitle = ParsedInfo("playlist_title")
                                          Dim PlaylistUploader = ParsedInfo("playlist_uploader")
                                          Dim PlaylistUploader_ID = ParsedInfo("playlist_uploader_id")
                                          Dim PlaylistInfo As New YoutubeVideo.YoutubePlaylist
                                          Try
                                              PlaylistInfo.Index = PlaylistIndex
                                          Catch
                                          End Try
                                          Try
                                              PlaylistInfo.Playlist_ID = PlaylistID
                                          Catch
                                          End Try
                                          Try
                                              PlaylistInfo.Title = PlaylistTitle
                                          Catch
                                          End Try
                                          Try
                                              PlaylistInfo.Uploader = PlaylistUploader
                                          Catch
                                          End Try
                                          Try
                                              PlaylistInfo.Uploader_ID = PlaylistUploader_ID
                                          Catch
                                          End Try
                                          'Chapter info
                                          Dim RawChapters = ParsedInfo("chapters")
                                          Dim Chapters As New List(Of YoutubeVideo.Chapter)
                                          If RawChapters IsNot Nothing Then
                                              For Each Chapter As Json.Linq.JToken In RawChapters
                                                  If Chapter Is Nothing Then Continue For
                                                  Dim ETime = Chapter("end_time")
                                                  Dim STime = Chapter("start_time")
                                                  Dim CTitle = Chapter("title")
                                                  Dim Chap As New YoutubeVideo.Chapter
                                                  Try
                                                      Chap.EndTime = TimeSpan.FromSeconds(ETime)
                                                  Catch
                                                  End Try
                                                  Try
                                                      Chap.StarTime = TimeSpan.FromSeconds(STime)
                                                  Catch
                                                  End Try
                                                  Try
                                                          Chap.Title = CTitle
                                                      Catch
                                                      End Try
                                                  Chapters.Add(Chap)
                                              Next
                                          End If
                                          Dim Title = ParsedInfo("title")
                                          Dim RawUploadDate = ParsedInfo("upload_date")
                                          Dim UploadDate As New Date(RawUploadDate.ToString.Substring(0, 4), RawUploadDate.ToString.Substring(4, 2), RawUploadDate.ToString.Substring(6, 2))
                                          Dim VideoCodec = ParsedInfo("vcodec")
                                          Dim VideoExtension = ParsedInfo("ext")
                                          Dim ViewCount = ParsedInfo("view_count")
                                          'Returning Info
                                          Dim Video = New YoutubeVideo
                                          With Video
                                              .JSONDump = Info
                                              .AlternateTitle = AltTitle
                                              .ThumbnailURL = ThumbnailURL
                                              .AudioCodec = AudioCodec
                                              .Author = Author
                                              .AuthorID = AuhorID
                                              .AuthorURL = AuthorURL
                                              .Categories = Categories
                                              .Creator = Creator
                                              .Description = Description
                                              .DislikeCount = DislikeCount?.ToString
                                              .Duration = Duration
                                              .FileName = FileName
                                              .FPS = FPS
                                              .FullTitle = FullTitle
                                              .LikeCount = LikeCount
                                              .Subtitles = Subtitles
                                              .Tags = Tags
                                              .Thumbnails = Thumbnails
                                              .Title = Title
                                              .UploadDate = UploadDate
                                              .VideoCodec = VideoCodec
                                              .VideoExtension = VideoExtension
                                              .ViewCount = ViewCount
                                              .Videos = DirectURLS
                                              .HQAudio = HQAudio
                                              .HQMixed = HQMixed
                                              .HQThumbnail = HQThumbnail
                                              .HQVideo = HQVideo
                                              .LQAudio = LQAudio
                                              .LQMixed = LQMixed
                                              .LQThumbnail = LQThumbnail
                                              .LQVideo = LQVideo
                                              .AudioOnly = AudioOnlyURLS
                                              .VideoOnly = VideoOnlyURLS
                                              .MixedOnly = MixedOnlyURLS
                                              .Chapters = Chapters
                                              .Properties = MediaInfo
                                              .Playlist = PlaylistInfo
                                              'Damn debugging
                                          End With
                                          Return Video
                                      Catch ex As Exception
                                          Utils.WriteConsoleError(ex)
                                      End Try
                                      Return Nothing
                                  End Function)
        End Function
        Public Async Function DumpAndManageVideo(URL As String, Optional SkipURLCheck As Boolean = False) As Task(Of YoutubeVideo)
            If CheckInternetConnection Then
                If SkipURLCheck = False Then
                    If IsYoutubeLink(URL) = False Then Return Nothing
                End If
                Return Await Task.Run(Async Function()
                                          Try
                                              Dim YoutubeDL As Process = Process.Start(New ProcessStartInfo(YoutubeDLLocation, "-j " & URL) With {.RedirectStandardOutput = True, .UseShellExecute = False, .WindowStyle = ProcessWindowStyle.Hidden, .CreateNoWindow = True})
                                              Dim Info = YoutubeDL.StandardOutput.ReadToEnd
                                              Return Await ParseYoutubeJson(Info)
                                          Catch ex As Exception
                                              Utils.WriteConsoleError(ex)
                                          End Try
                                          Return Nothing
                                      End Function)
            End If
            Return Nothing
        End Function
    End Class
End Namespace
