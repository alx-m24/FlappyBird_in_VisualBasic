Imports System.Threading

Module Program
    Structure Vector
        Public x As Double
        Public y As Double
    End Structure

    Class Player
        Public position As Vector
        Public velocity As Vector
        Public acceleration As Vector

        Sub Update()
            velocity.x += acceleration.x
            velocity.y += acceleration.y

            position.x += velocity.x
            position.y += velocity.y

            acceleration.x = 0
            acceleration.y = 0
        End Sub
    End Class

    Const worldSize As UInteger = 25
    Const playerYAcc As Double = 0.002
    Const pipeVel As Double = 0.03
    Dim halfGapSize As Integer = 4 / 2
    Dim pipeNum As UInteger = 2

    Dim world(worldSize, worldSize) As Char
    Dim player1 As New Player()
    Dim pipes(pipeNum) As Vector
    Dim keyThread As New Threading.Thread(AddressOf keyThreadProc)
    Dim alive As Boolean = True
    Dim delay As UInteger = 1

    Sub Main(args As String())
        Console.WriteLine("Welcome.")
start:
        Console.Write("Please enter the world gap size(Recommended: around 4): ")
        halfGapSize = Console.ReadLine() / 2
        Console.Write("Please enter the number of pipes(Recommended: around 2): ")
        pipeNum = Console.ReadLine()
        Console.WriteLine("For better visibility, I added a 1ms delay in the game loop. However, this makes the game choppy.")

        Console.Write("Do you still want to keep it?(y/n): ")
        Dim input As Char = Console.ReadLine()

        If (input = "n") Then
            delay = 0
        End If

        player1.position.x = 2

        Randomize()
        For i As Integer = 0 To pipeNum - 1
            pipes(i).x = worldSize / 2 + (i * ((worldSize / 2) / pipeNum))
            pipes(i).y = Int(((worldSize - halfGapSize - 1) * Rnd()) + halfGapSize)
        Next

        Console.Clear()
        Console.WriteLine(3)
        Threading.Thread.Sleep(1000)
        Console.WriteLine(2)
        Threading.Thread.Sleep(1000)
        Console.WriteLine(1)
        Threading.Thread.Sleep(1000)
        Console.WriteLine("GO!!!")

        keyThread.Start()

        While inBounds() And alive
            Call update()

            Call display()
            System.Threading.Thread.Sleep(delay)
        End While

        Console.Clear()
        Console.WriteLine("Game Over")
        Console.Write("Restart?(y/n): ")
        input = Console.ReadLine()
        If (input = "y") Then
            GoTo start
        End If
    End Sub

    Function inBounds() As Boolean
        Return player1.position.y < worldSize And player1.position.y >= 0 And player1.position.x < worldSize And player1.position.x >= 0
    End Function

    Sub update()
        player1.acceleration.y = playerYAcc
        player1.Update()

        For i As Integer = 0 To worldSize - 1
            For j As Integer = 0 To worldSize - 1
                world(i, j) = " "
            Next
        Next

        For i As Integer = 0 To pipeNum - 1
            pipes(i).x -= pipeVel
            If (pipes(i).x < 0.0) Then
                pipes(i).x = worldSize - 1
                pipes(i).y = Int(((worldSize - halfGapSize - 1) * Rnd()) + halfGapSize)
            End If

            Dim upperBound As Integer = pipes(i).y - halfGapSize
            Dim lowerBound As Integer = pipes(i).y + halfGapSize
            For j As Integer = 0 To upperBound - 1
                world(pipes(i).x, j) = "|"
            Next
            For j As Integer = lowerBound To worldSize - 1
                world(pipes(i).x, j) = "|"
            Next
        Next

        If (world(player1.position.x, player1.position.y) = "|") Then
            alive = False
            Return
        End If
        world(player1.position.x, player1.position.y) = "*"

    End Sub

    Sub display()
        Console.Clear()
        For i As Integer = 0 To worldSize - 1
            For j As Integer = 0 To worldSize - 1
                Console.Write(world(j, i) & " ")
            Next
            Console.WriteLine()
        Next

        For i As Integer = 0 To worldSize - 1
            Console.Write("__")
        Next
        Console.WriteLine()
    End Sub

    Sub keyThreadProc()
        While alive And inBounds()
            Dim key As Char = Console.ReadKey(True).KeyChar

            If key = " " Then
                player1.velocity.y = -0.15
            End If
        End While
    End Sub
End Module
